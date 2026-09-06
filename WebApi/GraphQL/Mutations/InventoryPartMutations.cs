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
            [Service] BlobStorageService blobStorageService,
            string? imageUrl,
            string name,
            int stockQuantity,
            decimal unitCost,
            decimal taxCost,
            decimal totalUnitCost,
            decimal basePrice,
            string currency,
            int minStockAlert,
            IFile? imageFile)
        {
            string finalImageUrl = imageUrl ?? "";

            if (imageFile != null)
            {
                var uid = Guid.NewGuid().ToString();
                var blobName = $"inventory/{uid}.JPEG";

                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                finalImageUrl = await blobStorageService.UploadImageAsync(memoryStream, blobName, "photos");
            }

            var newInventoryPart = new InventoryPart
            {
                imageUrl = finalImageUrl,
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
            [Service] BlobStorageService blobStorageService,
            int inventoryPartId,
            string? imageUrl,
            string name,
            int stockQuantity,
            decimal unitCost,
            decimal taxCost,
            decimal totalUnitCost,
            decimal basePrice,
            string currency,
            int minStockAlert,
            IFile? imageFile)
        {
            var part = await context.InventoryParts.FindAsync(inventoryPartId);

            if (part == null)
            {
                throw new GraphQLException("El repuesto no existe.");
            }

            string finalImageUrl = part.imageUrl;

            if (imageFile != null)
            {
                var uid = Guid.NewGuid().ToString();
                var blobName = $"inventory/{uid}.JPEG";

                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                finalImageUrl = await blobStorageService.UploadImageAsync(memoryStream, blobName, "photos");
            }
            else if (imageUrl != null)
            {
                finalImageUrl = imageUrl;
            }

            part.imageUrl = finalImageUrl;
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
