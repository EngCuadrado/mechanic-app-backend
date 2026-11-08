using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class VehicleRentalMutations
    {
        private static readonly string[] Periods = { "Diario", "Mensual", "Anual" };
        private static readonly string[] Statuses = { "Activo", "Finalizado", "Cancelado" };

        public async Task<VehicleRental> AddVehicleRentalAsync(
            [Service] AppDbContext context,
            int vehicleId,
            int companyId,
            DateTime startDate,
            DateTime? expectedEndDate,
            string billingPeriod,
            decimal rateAmount,
            string? contractUrl,
            string status = "Activo")
        {
            WorkOrderRules.RequireValid(billingPeriod, Periods, "Periodo de facturación");
            WorkOrderRules.RequireValid(status, Statuses, "Estado");
            if (rateAmount < 0) throw new GraphQLException("La tarifa no puede ser negativa.");
            if (!await context.Vehicles.AnyAsync(v => v.vehicleId == vehicleId)) throw new GraphQLException("El vehículo no existe.");
            if (!await context.Companies.AnyAsync(c => c.companyId == companyId)) throw new GraphQLException("La empresa no existe.");

            var rental = new VehicleRental
            {
                vehicleId = vehicleId, companyId = companyId, startDate = startDate,
                expectedEndDate = expectedEndDate, billingPeriod = billingPeriod,
                rateAmount = rateAmount, status = status, contractUrl = contractUrl,
                createdAt = DateTime.Now
            };
            context.VehicleRentals.Add(rental);
            await context.SaveChangesAsync();
            return rental;
        }

        // Optional: omitir conserva el valor; enviar null lo borra.
        public async Task<VehicleRental> UpdateVehicleRentalAsync(
            [Service] AppDbContext context,
            int id,
            DateTime? startDate,
            Optional<DateTime?> expectedEndDate,
            Optional<DateTime?> actualEndDate,
            string? billingPeriod,
            decimal? rateAmount,
            string? status,
            Optional<string?> contractUrl)
        {
            var rental = await context.VehicleRentals.FindAsync(id) ?? throw new GraphQLException("El alquiler no existe.");

            if (billingPeriod != null) { WorkOrderRules.RequireValid(billingPeriod, Periods, "Periodo de facturación"); rental.billingPeriod = billingPeriod; }
            if (status != null) { WorkOrderRules.RequireValid(status, Statuses, "Estado"); rental.status = status; }
            if (rateAmount.HasValue)
            {
                if (rateAmount < 0) throw new GraphQLException("La tarifa no puede ser negativa.");
                rental.rateAmount = rateAmount.Value;
            }
            if (startDate.HasValue) rental.startDate = startDate.Value;
            if (expectedEndDate.HasValue) rental.expectedEndDate = expectedEndDate.Value;
            if (actualEndDate.HasValue) rental.actualEndDate = actualEndDate.Value;
            if (contractUrl.HasValue) rental.contractUrl = contractUrl.Value;

            await context.SaveChangesAsync();
            return rental;
        }
    }
}
