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
    public class PromotionMutations
    {
        /// <summary>
        /// Crea una nueva promoción en el sistema.
        /// Si aplica a productos específicos, inserta en PromotionProduct usando una transacción.
        /// </summary>
        public async Task<MutationResult> CreatePromotionAsync(
            CreatePromotionInput input,
            [Service] AppDbContext context)
        {
            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(input.PromotionName))
                return new MutationResult(false, "El nombre de la promoción es obligatorio");

            if (input.DiscountValue <= 0)
                return new MutationResult(false, "El valor del descuento debe ser mayor a cero");

            var calculationType = input.CalculationType.ToUpper();
            if (calculationType != "PERCENTAGE" && calculationType != "FIXED_AMOUNT")
                return new MutationResult(false, "Tipo de cálculo no válido (PERCENTAGE o FIXED_AMOUNT)");

            // 2. Iniciar Transacción
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // 3. Crear la entidad Promotion
                var promotion = new Promotion
                {
                    PromotionName = input.PromotionName,
                    CalculationType = calculationType,
                    DiscountValue = input.DiscountValue,
                    StartDate = input.StartDate,
                    EndDate = input.EndDate,
                    IsActive = input.IsActive,
                    AppliesToAllProducts = input.AppliesToAllProducts
                };

                context.Promotions.Add(promotion);
                // Guardamos para obtener el promotion_id generado
                await context.SaveChangesAsync();

                // 4. Gestionar productos específicos si aplica
                if (!input.AppliesToAllProducts && input.ProductIds != null && input.ProductIds.Any())
                {
                    foreach (var productId in input.ProductIds)
                    {
                        // Verificar que el producto existe
                        var exists = await context.Products.AnyAsync(p => p.product_id == productId);
                        if (!exists)
                        {
                            await transaction.RollbackAsync();
                            return new MutationResult(false, $"El producto con ID {productId} no existe");
                        }

                        var promotionProduct = new PromotionProduct
                        {
                            PromotionId = promotion.PromotionId,
                            ProductId = productId
                        };
                        context.PromotionProducts.Add(promotionProduct);
                    }
                    await context.SaveChangesAsync();
                }
                else if (!input.AppliesToAllProducts && (input.ProductIds == null || !input.ProductIds.Any()))
                {
                    // Si no aplica a todos y no mandan productos, es un error lógico
                    await transaction.RollbackAsync();
                    return new MutationResult(false, "Si la promoción no aplica a todos los productos, debe especificar al menos uno.");
                }

                // 5. Confirmar transacción
                await transaction.CommitAsync();
                return new MutationResult(true, "Promoción creada exitosamente");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new MutationResult(false, $"Error al crear la promoción: {ex.Message}");
            }
        }
    }
}
