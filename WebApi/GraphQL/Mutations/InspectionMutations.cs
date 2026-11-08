using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.GraphQL.Inputs;
using WebApi.GraphQL.Payloads;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InspectionMutations
    {
        private static readonly string[] Types = { "Entrada", "Salida" };

        public async Task<VehicleInspection> CrearInspeccionAsync(
            [Service] AppDbContext context,
            int workOrderId,
            string inspectionType,
            int? mileage,
            string? inspectionNotes,
            string? damageMapUrl)
        {
            WorkOrderRules.RequireValid(inspectionType, Types, "Tipo de inspección");
            if (mileage < 0) throw new GraphQLException("El kilometraje no puede ser negativo.");
            var order = await WorkOrderSupport.GetOrderAsync(context, workOrderId);
            WorkOrderSupport.EnsureNotClosed(order);
            if (await context.VehicleInspections.AnyAsync(i => i.workOrderId == workOrderId && i.inspectionType == inspectionType))
                throw new GraphQLException($"La orden ya tiene una inspección de {inspectionType}.");

            var inspection = new VehicleInspection
            {
                workOrderId = workOrderId, inspectionType = inspectionType, mileage = mileage,
                inspectionNotes = inspectionNotes, damageMapUrl = damageMapUrl, createdAt = DateTime.Now
            };
            context.VehicleInspections.Add(inspection);
            await context.SaveChangesAsync(); // carrera => UQ_Inspections_WO_Type => DbErrorFilter
            return inspection;
        }

        public async Task<VehicleInspection> EditarInspeccionAsync(
            [Service] AppDbContext context,
            int vehicleInspectionId,
            Optional<int?> mileage,
            Optional<string?> inspectionNotes,
            Optional<string?> damageMapUrl)
        {
            var inspection = await context.VehicleInspections.FindAsync(vehicleInspectionId)
                             ?? throw new GraphQLException("La inspección no existe.");
            WorkOrderSupport.EnsureNotClosed(await WorkOrderSupport.GetOrderAsync(context, inspection.workOrderId));
            if (mileage.HasValue && mileage.Value < 0) throw new GraphQLException("El kilometraje no puede ser negativo.");

            if (mileage.HasValue) inspection.mileage = mileage.Value;
            if (inspectionNotes.HasValue) inspection.inspectionNotes = inspectionNotes.Value;
            if (damageMapUrl.HasValue) inspection.damageMapUrl = damageMapUrl.Value;
            await context.SaveChangesAsync();
            return inspection;
        }

        // Upsert en lote por (inspección, ítem) en una transacción. Todo o nada: si un valor es inválido no se guarda nada.
        // validarCompletitud = true actúa como "cerrar": todos los ítems activos con is_required deben tener valor.
        public async Task<InspectionForm> GuardarResultadosInspeccionAsync(
            [Service] AppDbContext context,
            int vehicleInspectionId,
            List<InspectionResultInput> results,
            bool validarCompletitud = false)
        {
            var inspection = await context.VehicleInspections.Include(i => i.Results)
                .FirstOrDefaultAsync(i => i.vehicleInspectionId == vehicleInspectionId)
                ?? throw new GraphQLException("La inspección no existe.");
            WorkOrderSupport.EnsureNotClosed(await WorkOrderSupport.GetOrderAsync(context, inspection.workOrderId));

            if (results.GroupBy(r => r.ItemId).Any(g => g.Count() > 1))
                throw new GraphQLException("Un mismo ítem aparece más de una vez en el lote.");
            var ids = results.Select(r => r.ItemId).ToList();
            var items = await context.InspectionChecklistItems.AsNoTracking()
                .Where(i => ids.Contains(i.itemId)).ToDictionaryAsync(i => i.itemId);

            var errors = new List<string>();
            foreach (var r in results)
            {
                if (!items.TryGetValue(r.ItemId, out var item)) { errors.Add($"Ítem {r.ItemId}: no existe."); continue; }
                if (!item.isActive) { errors.Add($"\"{item.name}\": el ítem está inactivo."); continue; }
                var error = InspectionRules.ValidateValue(item.inputType, item.options, r.Value);
                if (error != null) errors.Add($"\"{item.name}\": {error}.");
                if (r.Notes?.Length > 500) errors.Add($"\"{item.name}\": las notas superan 500 caracteres.");
            }
            if (errors.Count > 0) throw new GraphQLException(string.Join(" ", errors));

            using var transaction = await context.Database.BeginTransactionAsync();
            foreach (var r in results)
            {
                var value = string.IsNullOrWhiteSpace(r.Value) ? null : r.Value;
                var existing = inspection.Results.FirstOrDefault(x => x.itemId == r.ItemId);
                if (existing == null)
                    inspection.Results.Add(new VehicleInspectionResult { itemId = r.ItemId, value = value, notes = r.Notes });
                else { existing.value = value; existing.notes = r.Notes; }
            }
            await context.SaveChangesAsync();

            if (validarCompletitud)
            {
                var filled = inspection.Results.Where(x => !string.IsNullOrWhiteSpace(x.value)).Select(x => x.itemId).ToHashSet();
                var missing = await context.InspectionChecklistItems.AsNoTracking()
                    .Where(i => i.isActive && i.isRequired && !filled.Contains(i.itemId))
                    .Select(i => i.name).ToListAsync();
                if (missing.Count > 0)
                    throw new GraphQLException($"Faltan ítems obligatorios: {string.Join(", ", missing)}."); // la transacción se revierte
            }
            await transaction.CommitAsync();
            return await WorkOrderSupport.BuildFormAsync(context, vehicleInspectionId);
        }
    }
}
