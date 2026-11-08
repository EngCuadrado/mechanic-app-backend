using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.GraphQL.Mutations
{
    // Repuestos y servicios de una orden. El stock NO se toca aquí: lo mueve el trigger de work_order_parts.
    // Cada mutation recalcula los totales en la misma transacción y devuelve la orden completa.
    [ExtendObjectType(typeof(Mutation))]
    public class WorkOrderItemMutations
    {
        public async Task<WorkOrder> AgregarRepuestoOrdenAsync(
            [Service] AppDbContext context,
            int workOrderId,
            int partId,
            int quantity,
            decimal? totalUnitCost,
            decimal? unitPrice)
        {
            if (quantity <= 0) throw new GraphQLException("La cantidad debe ser mayor que 0.");
            if (totalUnitCost < 0 || unitPrice < 0) throw new GraphQLException("Los montos no pueden ser negativos.");
            var order = await WorkOrderSupport.GetOrderAsync(context, workOrderId);
            WorkOrderSupport.EnsureNotClosed(order);
            var part = await context.InventoryParts.AsNoTracking().FirstOrDefaultAsync(p => p.inventoryPartId == partId)
                       ?? throw new GraphQLException("El repuesto no existe.");
            if (!part.isActive) throw new GraphQLException("El repuesto está inactivo.");

            using var transaction = await context.Database.BeginTransactionAsync();
            context.WorkOrderParts.Add(new WorkOrderPart
            {
                workOrderId = workOrderId, partId = partId, quantity = quantity,
                totalUnitCost = totalUnitCost ?? part.totalUnitCost, // defaults del inventario, sobrescribibles
                unitPrice = unitPrice ?? part.basePrice
            });
            await context.SaveChangesAsync(); // stock insuficiente => "Stock insuficiente"
            await WorkOrderSupport.RecalculateTotalsAsync(context, order);
            await transaction.CommitAsync();
            return await WorkOrderSupport.LoadFullAsync(context, workOrderId);
        }

        public async Task<WorkOrder> EditarRepuestoOrdenAsync(
            [Service] AppDbContext context,
            int workOrderPartId,
            int? quantity,
            decimal? totalUnitCost,
            decimal? unitPrice)
        {
            if (quantity <= 0) throw new GraphQLException("La cantidad debe ser mayor que 0.");
            if (totalUnitCost < 0 || unitPrice < 0) throw new GraphQLException("Los montos no pueden ser negativos.");
            var line = await context.WorkOrderParts.FindAsync(workOrderPartId) ?? throw new GraphQLException("El repuesto de la orden no existe.");
            var order = await WorkOrderSupport.GetOrderAsync(context, line.workOrderId);
            WorkOrderSupport.EnsureNotClosed(order);

            using var transaction = await context.Database.BeginTransactionAsync();
            if (quantity.HasValue) line.quantity = quantity.Value;
            if (totalUnitCost.HasValue) line.totalUnitCost = totalUnitCost.Value;
            if (unitPrice.HasValue) line.unitPrice = unitPrice.Value;
            await context.SaveChangesAsync();
            await WorkOrderSupport.RecalculateTotalsAsync(context, order);
            await transaction.CommitAsync();
            return await WorkOrderSupport.LoadFullAsync(context, order.workOrderId);
        }

        // Permitido también en órdenes Cancelado (devuelve el stock); bloqueado en Entregado.
        public async Task<WorkOrder> QuitarRepuestoOrdenAsync([Service] AppDbContext context, int workOrderPartId)
        {
            var line = await context.WorkOrderParts.FindAsync(workOrderPartId) ?? throw new GraphQLException("El repuesto de la orden no existe.");
            var order = await WorkOrderSupport.GetOrderAsync(context, line.workOrderId);
            if (order.currentStatus == "Entregado") throw new GraphQLException("La orden está Entregado y no admite cambios.");

            using var transaction = await context.Database.BeginTransactionAsync();
            context.WorkOrderParts.Remove(line);
            await context.SaveChangesAsync();
            await WorkOrderSupport.RecalculateTotalsAsync(context, order);
            await transaction.CommitAsync();
            return await WorkOrderSupport.LoadFullAsync(context, order.workOrderId);
        }

        public async Task<WorkOrder> AgregarServicioOrdenAsync(
            [Service] AppDbContext context,
            int workOrderId,
            string description,
            decimal internalCost,
            decimal price,
            int? mechanicId)
        {
            if (string.IsNullOrWhiteSpace(description)) throw new GraphQLException("La descripción es obligatoria.");
            if (internalCost < 0 || price < 0) throw new GraphQLException("Los montos no pueden ser negativos.");
            var order = await WorkOrderSupport.GetOrderAsync(context, workOrderId);
            WorkOrderSupport.EnsureNotClosed(order);
            if (mechanicId.HasValue && !await context.Mechanics.AnyAsync(m => m.MechanicId == mechanicId.Value))
                throw new GraphQLException("El mecánico no existe.");

            using var transaction = await context.Database.BeginTransactionAsync();
            context.WorkOrderServices.Add(new WorkOrderService
            {
                workOrderId = workOrderId, mechanicId = mechanicId,
                description = description.Trim(), internalCost = internalCost, price = price
            });
            await context.SaveChangesAsync();
            await WorkOrderSupport.RecalculateTotalsAsync(context, order);
            await transaction.CommitAsync();
            return await WorkOrderSupport.LoadFullAsync(context, workOrderId);
        }

        public async Task<WorkOrder> EditarServicioOrdenAsync(
            [Service] AppDbContext context,
            int workOrderServiceId,
            string? description,
            decimal? internalCost,
            decimal? price,
            Optional<int?> mechanicId)
        {
            if (internalCost < 0 || price < 0) throw new GraphQLException("Los montos no pueden ser negativos.");
            var line = await context.WorkOrderServices.FindAsync(workOrderServiceId) ?? throw new GraphQLException("El servicio de la orden no existe.");
            var order = await WorkOrderSupport.GetOrderAsync(context, line.workOrderId);
            WorkOrderSupport.EnsureNotClosed(order);
            if (description != null && string.IsNullOrWhiteSpace(description)) throw new GraphQLException("La descripción es obligatoria.");
            if (mechanicId.HasValue && mechanicId.Value.HasValue
                && !await context.Mechanics.AnyAsync(m => m.MechanicId == mechanicId.Value.Value))
                throw new GraphQLException("El mecánico no existe.");

            using var transaction = await context.Database.BeginTransactionAsync();
            if (description != null) line.description = description.Trim();
            if (internalCost.HasValue) line.internalCost = internalCost.Value;
            if (price.HasValue) line.price = price.Value;
            if (mechanicId.HasValue) line.mechanicId = mechanicId.Value;
            await context.SaveChangesAsync();
            await WorkOrderSupport.RecalculateTotalsAsync(context, order);
            await transaction.CommitAsync();
            return await WorkOrderSupport.LoadFullAsync(context, order.workOrderId);
        }

        public async Task<WorkOrder> QuitarServicioOrdenAsync([Service] AppDbContext context, int workOrderServiceId)
        {
            var line = await context.WorkOrderServices.FindAsync(workOrderServiceId) ?? throw new GraphQLException("El servicio de la orden no existe.");
            var order = await WorkOrderSupport.GetOrderAsync(context, line.workOrderId);
            WorkOrderSupport.EnsureNotClosed(order);

            using var transaction = await context.Database.BeginTransactionAsync();
            context.WorkOrderServices.Remove(line);
            await context.SaveChangesAsync();
            await WorkOrderSupport.RecalculateTotalsAsync(context, order);
            await transaction.CommitAsync();
            return await WorkOrderSupport.LoadFullAsync(context, order.workOrderId);
        }
    }
}
