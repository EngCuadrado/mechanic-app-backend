using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InventoryPartMutations
    {
        public async Task<InventoryPart> AddInventoryPartAsync(
            [Service] AppDbContext context,
            string? imageUrl,
            string name,
            int stockQuantity,
            decimal unitCost,
            decimal taxCost,
            decimal totalUnitCost,
            decimal basePrice,
            string currency,
            int minStockAlert)
        {
            var newInventoryPart = new InventoryPart
            {
                imageUrl = imageUrl,
                name = name,
                stockQuantity = stockQuantity,
                unitCost = unitCost,
                taxCost = taxCost,
                totalUnitCost = totalUnitCost,
                basePrice = basePrice,
                currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency,
                minStockAlert = minStockAlert
            };

            context.InventoryParts.Add(newInventoryPart);
            await context.SaveChangesAsync();

            return newInventoryPart;
        }

        public async Task<InventoryPart> UpdateInventoryPartAsync(
            [Service] AppDbContext context,
            int inventoryPartId,
            string? imageUrl,
            string name,
            int stockQuantity,
            decimal unitCost,
            decimal taxCost,
            decimal totalUnitCost,
            decimal basePrice,
            string currency,
            int minStockAlert)
        {
            var part = await context.InventoryParts.FindAsync(inventoryPartId);

            if (part == null)
            {
                throw new GraphQLException("El repuesto no existe.");
            }

            part.imageUrl = imageUrl;
            part.name = name;
            part.stockQuantity = stockQuantity;
            part.unitCost = unitCost;
            part.taxCost = taxCost;
            part.totalUnitCost = totalUnitCost;
            part.basePrice = basePrice;
            part.currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency;
            part.minStockAlert = minStockAlert;

            await context.SaveChangesAsync();

            return part;
        }

        public async Task<bool> DeleteInventoryPartAsync(
            [Service] AppDbContext context,
            int inventoryPartId)
        {
            var part = await context.InventoryParts.FindAsync(inventoryPartId);

            if (part == null)
            {
                throw new GraphQLException("El repuesto no existe.");
            }

            context.InventoryParts.Remove(part);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
