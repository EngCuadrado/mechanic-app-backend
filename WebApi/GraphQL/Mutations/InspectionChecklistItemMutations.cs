using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InspectionChecklistItemMutations
    {
        private static bool UsesOptions(string inputType) => InspectionRules.UsesOptions(inputType);

        private static async Task EnsureCategoryAsync(AppDbContext context, int? categoryId)
        {
            if (categoryId.HasValue && !await context.InspectionCategories.AnyAsync(c => c.categoryId == categoryId.Value))
                throw new GraphQLException("La categoría no existe.");
        }

        public async Task<InspectionChecklistItem> AddInspectionChecklistItemAsync(
            [Service] AppDbContext context,
            string name,
            string inputType,
            List<string>? options,
            int? categoryId,
            bool isRequired = true,
            string appliesTo = "Ambas")
        {
            WorkOrderRules.RequireValid(appliesTo, InspectionRules.AppliesToValues, "appliesTo");
            name = name?.Trim() ?? "";
            if (name.Length == 0) throw new GraphQLException("El nombre del ítem es obligatorio.");
            inputType = inputType?.Trim() ?? "";
            var cleanOptions = InspectionRules.ValidateOptions(inputType, options);
            await EnsureCategoryAsync(context, categoryId);

            var item = new InspectionChecklistItem
            {
                name = name,
                inputType = inputType,
                options = cleanOptions,
                categoryId = categoryId,
                isRequired = isRequired,
                appliesTo = appliesTo,
                isActive = true
            };
            context.InspectionChecklistItems.Add(item);
            await context.SaveChangesAsync();

            return item;
        }

        // options y categoryId usan Optional: omitirlos conserva el valor; enviar null lo borra.
        public async Task<InspectionChecklistItem> UpdateInspectionChecklistItemAsync(
            [Service] AppDbContext context,
            int itemId,
            string? name,
            string? inputType,
            Optional<List<string>?> options,
            Optional<int?> categoryId,
            bool? isRequired,
            string? appliesTo)
        {
            var item = await context.InspectionChecklistItems.FindAsync(itemId);
            if (item == null) throw new GraphQLException("El ítem no existe.");

            if (name != null)
            {
                name = name.Trim();
                if (name.Length == 0) throw new GraphQLException("El nombre del ítem es obligatorio.");
                item.name = name;
            }

            var finalType = inputType?.Trim() ?? item.inputType;
            // Si cambia el tipo y no envían options, se descartan las anteriores solo cuando el nuevo tipo no las usa.
            var finalOptions = options.HasValue ? options.Value
                : UsesOptions(finalType) ? item.options : null;
            item.inputType = finalType;
            item.options = InspectionRules.ValidateOptions(finalType, finalOptions);

            if (categoryId.HasValue)
            {
                await EnsureCategoryAsync(context, categoryId.Value);
                item.categoryId = categoryId.Value;
            }
            if (isRequired.HasValue) item.isRequired = isRequired.Value;
            if (appliesTo != null)
            {
                WorkOrderRules.RequireValid(appliesTo, InspectionRules.AppliesToValues, "appliesTo");
                item.appliesTo = appliesTo;
            }

            await context.SaveChangesAsync();

            return item;
        }

        public async Task<InspectionChecklistItem> ToggleInspectionChecklistItemStatusAsync(
            [Service] AppDbContext context,
            int itemId)
        {
            var item = await context.InspectionChecklistItems.FindAsync(itemId);
            if (item == null) throw new GraphQLException("El ítem no existe.");

            item.isActive = !item.isActive;
            await context.SaveChangesAsync();

            return item;
        }
    }
}
