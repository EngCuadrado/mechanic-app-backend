using System.Security.Claims;
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
            decimal unitCost,
            decimal taxCost,
            decimal totalUnitCost,
            decimal basePrice,
            string currency,
            int minStockAlert,
            IFile? imageFile,
            ClaimsPrincipal? user,
            int initialStock = 0)
        {
            if (initialStock < 0) throw new GraphQLException("El stock inicial no puede ser negativo.");
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
                unitCost = unitCost,
                taxCost = taxCost,
                totalUnitCost = totalUnitCost,
                basePrice = basePrice,
                currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency,
                minStockAlert = minStockAlert,
                isActive = true
            };

            // El stock nace en 0 (EF no lo escribe); el saldo inicial entra como movimiento 'Inicial'.
            using var transaction = await context.Database.BeginTransactionAsync();
            context.InventoryParts.Add(newInventoryPart);
            await context.SaveChangesAsync();

            if (initialStock > 0)
            {
                context.InventoryMovements.Add(new InventoryMovement
                {
                    partId = newInventoryPart.inventoryPartId,
                    quantity = initialStock,
                    movementType = "Inicial",
                    notes = "Stock inicial",
                    createdBy = WorkOrderSupport.CurrentUserId(user),
                    createdAt = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
            await transaction.CommitAsync();

            await context.Entry(newInventoryPart).ReloadAsync(); // trae el stock calculado por el trigger
            return newInventoryPart;
        }

        public async Task<InventoryPart> UpdateInventoryPartAsync(
            [Service] AppDbContext context,
            [Service] BlobStorageService blobStorageService,
            int inventoryPartId,
            string? imageUrl,
            string name,
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
    public async Task<InventoryPart> ToggleInventoryPartStatusAsync(
        [Service] AppDbContext context,
        int inventoryPartId)
    {
        var part = await context.InventoryParts.FindAsync(inventoryPartId);

        if (part == null)
        {
            throw new GraphQLException("El repuesto no existe.");
        }

        part.isActive = !part.isActive;

        await context.SaveChangesAsync();

        return part;
    }
}
}
