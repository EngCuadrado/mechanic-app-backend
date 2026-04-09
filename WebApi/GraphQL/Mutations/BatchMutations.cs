using WebApi.Data;
using WebApi.GraphQL.Inputs;
using WebApi.Models;
using WebApi.GraphQL.Payloads;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class BatchMutations
    {
        public async Task<MutationResult> AddBatchAsync(
            AddBatchInput input,
            [Service] AppDbContext context)
        {
            var product = await context.Products.FindAsync(input.ProductId);
            if (product == null)
            {
                return new MutationResult(false, "El producto especificado no existe.");
            }

            // Verificar si el código de lote ya existe para este producto
            var batchExists = await context.Batches.AnyAsync(b => b.product_id == input.ProductId && b.batch_code == input.BatchCode);
            if (batchExists)
            {
                return new MutationResult(false, $"El código de lote '{input.BatchCode}' ya existe para este producto.");
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var batch = new Batch
                {
                    product_id = input.ProductId,
                    batch_code = input.BatchCode,
                    expiration_date = input.ExpirationDate,
                    initial_quantity_units = input.QuantityUnits,
                    current_quantity_units = input.QuantityUnits,
                    is_active = true,
                    created_at = DateTime.Now
                };

                context.Batches.Add(batch);
                
                // Actualizar el stock total del producto
                product.stock_units += input.QuantityUnits;
                context.Products.Update(product);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new MutationResult(true, "Lote agregado exitosamente y stock del producto actualizado.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new MutationResult(false, $"Error al agregar el lote: {ex.Message}");
            }
        }
    }
}
