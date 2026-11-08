using System.Security.Claims;
using HotChocolate;
using HotChocolate.Types;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InventoryMovementMutations
    {
        // Solo Compra, Ajuste e Inicial. Uso/Devolucion los genera el trigger de work_order_parts.
        // El trigger de la BD suma la cantidad a inventory_parts.stock_quantity.
        public async Task<InventoryMovement> RegistrarMovimientoAsync(
            [Service] AppDbContext context,
            ClaimsPrincipal? user,
            int partId,
            string movementType,
            int quantity,
            decimal? unitCost,
            string? notes)
        {
            MovementRules.Validate(movementType, quantity, unitCost);
            if (notes?.Length > 500) throw new GraphQLException("Las notas no pueden superar 500 caracteres.");
            var part = await context.InventoryParts.FindAsync(partId) ?? throw new GraphQLException("El repuesto no existe.");

            var movement = new InventoryMovement
            {
                partId = partId, quantity = quantity, movementType = movementType,
                unitCost = unitCost, notes = notes,
                createdBy = WorkOrderSupport.CurrentUserId(user), createdAt = DateTime.Now
            };
            context.InventoryMovements.Add(movement);
            await context.SaveChangesAsync(); // stock negativo => CK_inventory_parts_stock => "Stock insuficiente"

            await context.Entry(part).ReloadAsync(); // stock actualizado por el trigger
            return movement;
        }
    }
}
