using System.Security.Claims;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.GraphQL.Payloads;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.GraphQL.Mutations
{
    // Helpers compartidos por las mutations de órdenes (no es un tipo GraphQL).
    internal static class WorkOrderSupport
    {
        // Usuario del JWT (claim "userId"); null si la petición no trae token.
        public static int? CurrentUserId(ClaimsPrincipal? user) =>
            int.TryParse(user?.FindFirst("userId")?.Value, out var id) ? id : null;

        public static async Task<WorkOrder> GetOrderAsync(AppDbContext context, int workOrderId) =>
            await context.WorkOrders.FindAsync(workOrderId) ?? throw new GraphQLException("La orden no existe.");

        public static void EnsureNotClosed(WorkOrder order)
        {
            if (WorkOrderRules.IsClosed(order.currentStatus))
                throw new GraphQLException($"La orden está {order.currentStatus} y no admite cambios.");
        }

        // Suma partes (cantidad × costo/precio) y servicios; llamar dentro de la transacción de la mutation.
        public static async Task RecalculateTotalsAsync(AppDbContext context, WorkOrder order)
        {
            var parts = await context.WorkOrderParts.AsNoTracking()
                .Where(p => p.workOrderId == order.workOrderId)
                .Select(p => new { p.quantity, p.totalUnitCost, p.unitPrice }).ToListAsync();
            var services = await context.WorkOrderServices.AsNoTracking()
                .Where(s => s.workOrderId == order.workOrderId)
                .Select(s => new { s.internalCost, s.price }).ToListAsync();

            var (cost, billed) = WorkOrderRules.Totals(
                parts.Select(p => (p.quantity, p.totalUnitCost, p.unitPrice)),
                services.Select(s => (s.internalCost ?? 0, s.price ?? 0)));
            order.totalInternalCost = cost;
            order.totalBilledPrice = billed;
            await context.SaveChangesAsync();
        }

        // Orden completa y fresca (sin tracking) para devolver al cliente.
        public static Task<WorkOrder> LoadFullAsync(AppDbContext context, int workOrderId) =>
            context.WorkOrders.AsNoTracking().AsSplitQuery()
                .Include(o => o.Vehicle).ThenInclude(v => v!.Model).ThenInclude(m => m!.Brand)
                .Include(o => o.Company).Include(o => o.Creator)
                .Include(o => o.Parts).ThenInclude(p => p.Part)
                .Include(o => o.Services).ThenInclude(s => s.Mechanic)
                .Include(o => o.Photos)
                .Include(o => o.Inspections)
                .FirstAsync(o => o.workOrderId == workOrderId);

        public static async Task<InspectionForm> BuildFormAsync(AppDbContext context, int? inspectionId, string? inspectionType = null)
        {
            VehicleInspection? inspection = null;
            if (inspectionId.HasValue)
                inspection = await context.VehicleInspections.AsNoTracking().Include(i => i.Results)
                    .FirstOrDefaultAsync(i => i.vehicleInspectionId == inspectionId.Value)
                    ?? throw new GraphQLException("La inspección no existe.");

            // Con inspección, manda su tipo; sin ella, el tipo pedido; sin ninguno, todos los ítems.
            var type = inspection?.inspectionType ?? inspectionType;
            if (type != null) WorkOrderRules.RequireValid(type, new[] { "Entrada", "Salida" }, "Tipo de inspección");

            var results = inspection?.Results.ToDictionary(r => r.itemId) ?? new();
            var items = await context.InspectionChecklistItems.AsNoTracking().Include(i => i.Category)
                .Where(i => (i.isActive && (type == null || i.appliesTo == "Ambas" || i.appliesTo == type))
                            || results.Keys.Contains(i.itemId)).ToListAsync();

            var groups = items
                .GroupBy(i => i.categoryId) // por id: con AsNoTracking cada ítem trae su propia instancia de Category
                .OrderBy(g => g.Key == null ? int.MaxValue : g.First().Category!.sortOrder).ThenBy(g => g.Key ?? int.MaxValue)
                .Select(g => new InspectionFormGroup(g.First().Category, g.OrderBy(i => i.itemId).Select(i =>
                    new InspectionFormItem(i,
                        results.TryGetValue(i.itemId, out var r) ? r.value : null,
                        results.TryGetValue(i.itemId, out r) ? r.notes : null)).ToList()))
                .ToList();
            return new InspectionForm(inspection, groups);
        }
    }
}
