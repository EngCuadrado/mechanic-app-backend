using WebApi.Data;
using WebApi.Models;
using WebApi.GraphQL.Payloads;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    public record CreatePaymentMethodInput(string Name);
    public record UpdatePaymentMethodInput(int PaymentMethodId, string Name);

    [ExtendObjectType(typeof(Mutation))]
    public class PaymentMethodMutations
    {
        public async Task<MutationResult> CreatePaymentMethodAsync(
            CreatePaymentMethodInput input,
            [Service] AppDbContext context)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
                return new MutationResult(false, "El nombre del método de pago es obligatorio");

            bool exists = await context.PaymentMethods.AnyAsync(p => p.Name.ToLower() == input.Name.ToLower());
            if (exists)
                return new MutationResult(false, "El método de pago ya existe");

            var pm = new PaymentMethod { Name = input.Name };

            try
            {
                context.PaymentMethods.Add(pm);
                await context.SaveChangesAsync();
                return new MutationResult(true, "Método de pago creado");
            }
            catch (Exception ex)
            {
                return new MutationResult(false, $"Error: {ex.Message}");
            }
        }

        public async Task<MutationResult> UpdatePaymentMethodAsync(
            UpdatePaymentMethodInput input,
            [Service] AppDbContext context)
        {
            var pm = await context.PaymentMethods.FindAsync(input.PaymentMethodId);
            if (pm == null)
                return new MutationResult(false, "Método de pago no encontrado");

            pm.Name = input.Name;

            try
            {
                await context.SaveChangesAsync();
                return new MutationResult(true, "Método de pago actualizado");
            }
            catch (Exception ex)
            {
                return new MutationResult(false, $"Error: {ex.Message}");
            }
        }
    }
}
