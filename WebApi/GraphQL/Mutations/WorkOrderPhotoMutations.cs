using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class WorkOrderPhotoMutations
    {
        private static readonly string[] Stages = { "Antes", "Despues", "Otro" };

        // Acepta photoUrl (ya subida) o photoFile (se sube a Azure Blob igual que las imágenes de inventario).
        public async Task<WorkOrderPhoto> RegistrarFotoOrdenAsync(
            [Service] AppDbContext context,
            [Service] BlobStorageService blobStorageService,
            int workOrderId,
            string stage,
            string? category,
            int? vehicleInspectionId,
            string? photoUrl,
            IFile? photoFile)
        {
            WorkOrderRules.RequireValid(stage, Stages, "Etapa");
            if (string.IsNullOrWhiteSpace(photoUrl) && photoFile == null)
                throw new GraphQLException("Debe enviar photoUrl o photoFile.");
            var order = await WorkOrderSupport.GetOrderAsync(context, workOrderId);
            if (order.currentStatus == "Entregado") throw new GraphQLException("La orden está Entregado: sus fotos son evidencia y no se modifican.");
            if (vehicleInspectionId.HasValue && !await context.VehicleInspections
                    .AnyAsync(i => i.vehicleInspectionId == vehicleInspectionId.Value && i.workOrderId == workOrderId))
                throw new GraphQLException("La inspección no pertenece a esta orden.");

            var url = photoUrl;
            if (photoFile != null)
            {
                using var ms = new MemoryStream();
                await photoFile.CopyToAsync(ms);
                url = await blobStorageService.UploadImageAsync(ms, $"workorders/{workOrderId}/{Guid.NewGuid()}.JPEG", "photos");
            }

            var photo = new WorkOrderPhoto
            {
                workOrderId = workOrderId, vehicleInspectionId = vehicleInspectionId,
                stage = stage, photoUrl = url!, category = category, uploadedAt = DateTime.Now
            };
            context.WorkOrderPhotos.Add(photo);
            await context.SaveChangesAsync();
            return photo;
        }

        public async Task<bool> EliminarFotoOrdenAsync([Service] AppDbContext context, int workOrderPhotoId)
        {
            var photo = await context.WorkOrderPhotos.FindAsync(workOrderPhotoId) ?? throw new GraphQLException("La foto no existe.");
            var order = await WorkOrderSupport.GetOrderAsync(context, photo.workOrderId);
            if (order.currentStatus == "Entregado") throw new GraphQLException("La orden está Entregado: sus fotos son evidencia y no se pueden eliminar.");

            context.WorkOrderPhotos.Remove(photo);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
