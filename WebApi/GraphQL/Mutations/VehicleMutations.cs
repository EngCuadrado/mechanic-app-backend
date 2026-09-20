using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class VehicleMutations
    {
        public async Task<Vehicle> AddVehicleAsync(
            [Service] AppDbContext context,
            int? companyId,
            int modelId,
            string plate,
            string vin,
            string chassisNumber,
            int year,
            int currentMilage,
            DateOnly nextMaintenanceDate,
            int nextMaintenanceMilage,
            DateOnly insuranceExpirationDate,
            DateOnly mechanicalInspectionExpiration,
            DateOnly emissionsInspectionExpiration,
            VehicleStatuses status)
        {
            var newVehicle = new Vehicle
            {
                companyId = companyId,
                modelId = modelId,
                plate = plate,
                vin = vin,
                chassisNumber = chassisNumber,
                year = year,
                currentMilage = currentMilage,
                nextMaintenanceDate = nextMaintenanceDate,
                nextMaintenanceMilage = nextMaintenanceMilage,
                insuranceExpirationDate = insuranceExpirationDate,
                mechanicalInspectionExpiration = mechanicalInspectionExpiration,
                emissionsInspectionExpiration = emissionsInspectionExpiration,
                status = status
            };

            context.Vehicles.Add(newVehicle);
            try 
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new GraphQLException($"Error al guardar: {innerMessage}");
            }

            return newVehicle;
        }

        public async Task<Vehicle> UpdateVehicleAsync(
            [Service] AppDbContext context,
            int vehicleId,
            int? companyId,
            int? modelId,
            string? plate,
            string? vin,
            string? chassisNumber,
            int? year,
            int? currentMilage,
            DateOnly? nextMaintenanceDate,
            int? nextMaintenanceMilage,
            DateOnly? insuranceExpirationDate,
            DateOnly? mechanicalInspectionExpiration,
            DateOnly? emissionsInspectionExpiration,
            VehicleStatuses? status)
        {
            var vehicle = await context.Vehicles.FindAsync(vehicleId);

            if (vehicle == null)
            {
                throw new GraphQLException("El vehículo no existe.");
            }

            if (companyId.HasValue) vehicle.companyId = companyId.Value;
            if (modelId.HasValue) vehicle.modelId = modelId.Value;
            if (!string.IsNullOrEmpty(plate)) vehicle.plate = plate;
            if (!string.IsNullOrEmpty(vin)) vehicle.vin = vin;
            if (!string.IsNullOrEmpty(chassisNumber)) vehicle.chassisNumber = chassisNumber;
            if (year.HasValue) vehicle.year = year.Value;
            if (currentMilage.HasValue) vehicle.currentMilage = currentMilage.Value;
            
            if (nextMaintenanceDate.HasValue) 
                vehicle.nextMaintenanceDate = nextMaintenanceDate.Value;
            
            if (nextMaintenanceMilage.HasValue) vehicle.nextMaintenanceMilage = nextMaintenanceMilage.Value;
            
            if (insuranceExpirationDate.HasValue) 
                vehicle.insuranceExpirationDate = insuranceExpirationDate.Value;
                
            if (mechanicalInspectionExpiration.HasValue) 
                vehicle.mechanicalInspectionExpiration = mechanicalInspectionExpiration.Value;
                
            if (emissionsInspectionExpiration.HasValue) 
                vehicle.emissionsInspectionExpiration = emissionsInspectionExpiration.Value;
                
            if (status.HasValue) vehicle.status = status.Value;

            await context.SaveChangesAsync();

            return vehicle;
        }

        public async Task<bool> DeleteVehicleAsync(
            [Service] AppDbContext context,
            int vehicleId)
        {
            var vehicle = await context.Vehicles.FindAsync(vehicleId);

            if (vehicle == null)
            {
                throw new GraphQLException("El vehículo no existe.");
            }

            vehicle.status = VehicleStatuses.FUERA_DE_SERVICIO;
            await context.SaveChangesAsync();

            return true;
        }
    }
}
