using System.Security.Claims;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class WorkOrderMutations
    {
        private static readonly string[] OpenStatuses = { "Recibido", "En proceso", "Listo" };

        public async Task<WorkOrder> CrearOrdenAsync(
            [Service] AppDbContext context,
            ClaimsPrincipal? user,
            int vehicleId,
            string type,
            string? reportedIssue,
            string? notes,
            string? dropoffDriverName,
            string? pickupDriverName,
            int? rentalId)
        {
            WorkOrderRules.RequireValid(type, WorkOrderRules.Types, "Tipo de orden");
            var vehicle = await context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.vehicleId == vehicleId)
                          ?? throw new GraphQLException("El vehículo no existe.");
            if (await context.WorkOrders.AnyAsync(o => o.vehicleId == vehicleId && OpenStatuses.Contains(o.currentStatus)))
                throw new GraphQLException("El vehículo ya tiene una orden abierta.");
            if (rentalId.HasValue && !await context.VehicleRentals.AnyAsync(r => r.id == rentalId.Value))
                throw new GraphQLException("El alquiler no existe.");

            var order = new WorkOrder
            {
                vehicleId = vehicleId,
                companyId = vehicle.companyId, // snapshot de quién tiene el vehículo ahora
                rentalId = rentalId,
                type = type,
                currentStatus = "Recibido",
                reportedIssue = reportedIssue,
                notes = notes,
                dropoffDriverName = dropoffDriverName,
                pickupDriverName = pickupDriverName,
                createdBy = WorkOrderSupport.CurrentUserId(user),
                entryTimestamp = WorkOrderRules.Now(),
                statusChangedAt = WorkOrderRules.Now()
            };
            context.WorkOrders.Add(order);
            await context.SaveChangesAsync(); // carrera con otra orden abierta => UX_WO_OneOpenPerVehicle => DbErrorFilter

            return await WorkOrderSupport.LoadFullAsync(context, order.workOrderId);
        }

        // Datos generales. Omitir un campo lo conserva; enviar null lo borra.
        public async Task<WorkOrder> ActualizarOrdenAsync(
            [Service] AppDbContext context,
            int workOrderId,
            Optional<string?> reportedIssue,
            Optional<string?> notes,
            Optional<string?> dropoffDriverName,
            Optional<string?> pickupDriverName,
            Optional<string?> dropoffSignatureUrl,
            Optional<string?> pickupSignatureUrl)
        {
            var order = await WorkOrderSupport.GetOrderAsync(context, workOrderId);
            WorkOrderSupport.EnsureNotClosed(order);

            if (reportedIssue.HasValue) order.reportedIssue = reportedIssue.Value;
            if (notes.HasValue) order.notes = notes.Value;
            if (dropoffDriverName.HasValue) order.dropoffDriverName = dropoffDriverName.Value;
            if (pickupDriverName.HasValue) order.pickupDriverName = pickupDriverName.Value;
            if (dropoffSignatureUrl.HasValue) order.dropoffSignatureUrl = dropoffSignatureUrl.Value;
            if (pickupSignatureUrl.HasValue) order.pickupSignatureUrl = pickupSignatureUrl.Value;

            await context.SaveChangesAsync();
            return await WorkOrderSupport.LoadFullAsync(context, workOrderId);
        }

        // Recibido -> En proceso -> Listo -> Entregado. "Cancelado" se delega en cancelarOrden.
        public async Task<WorkOrder> CambiarEstadoOrdenAsync(
            [Service] AppDbContext context,
            int workOrderId,
            string newStatus)
        {
            WorkOrderRules.RequireValid(newStatus, WorkOrderRules.Statuses, "Estado");
            if (newStatus == "Cancelado") return await CancelAsync(context, workOrderId);

            var order = await WorkOrderSupport.GetOrderAsync(context, workOrderId);
            if (!WorkOrderRules.CanTransition(order.currentStatus, newStatus))
                throw new GraphQLException($"Transición no permitida: {order.currentStatus} → {newStatus}.");

            order.currentStatus = newStatus;
            order.statusChangedAt = WorkOrderRules.Now();
            if (newStatus == "Entregado") order.exitTimestamp = WorkOrderRules.Now();
            await context.SaveChangesAsync();
            return await WorkOrderSupport.LoadFullAsync(context, workOrderId);
        }

        // Borra los repuestos de la orden (el trigger devuelve el stock) y luego marca Cancelado, en UNA transacción.
        // Los servicios se conservan; los totales se recalculan.
        public Task<WorkOrder> CancelarOrdenAsync([Service] AppDbContext context, int workOrderId) =>
            CancelAsync(context, workOrderId);

        private static async Task<WorkOrder> CancelAsync(AppDbContext context, int workOrderId)
        {
            var order = await WorkOrderSupport.GetOrderAsync(context, workOrderId);
            if (!WorkOrderRules.CanTransition(order.currentStatus, "Cancelado"))
                throw new GraphQLException($"No se puede cancelar una orden {order.currentStatus}.");

            using var transaction = await context.Database.BeginTransactionAsync();
            var parts = await context.WorkOrderParts.Where(p => p.workOrderId == workOrderId).ToListAsync();
            context.WorkOrderParts.RemoveRange(parts);
            order.currentStatus = "Cancelado";
            order.statusChangedAt = WorkOrderRules.Now();
            await context.SaveChangesAsync();
            await WorkOrderSupport.RecalculateTotalsAsync(context, order);
            await transaction.CommitAsync();

            return await WorkOrderSupport.LoadFullAsync(context, workOrderId);
        }
    }
}
