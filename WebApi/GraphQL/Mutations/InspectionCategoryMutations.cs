using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InspectionCategoryMutations
    {
        public async Task<InspectionCategory> AddInspectionCategoryAsync(
            [Service] AppDbContext context,
            string name,
            int sortOrder = 0)
        {
            name = name?.Trim() ?? "";
            if (name.Length == 0) throw new GraphQLException("El nombre de la categoría es obligatorio.");
            if (await context.InspectionCategories.AnyAsync(c => c.name == name))
                throw new GraphQLException("Ya existe una categoría con ese nombre.");

            var category = new InspectionCategory { name = name, sortOrder = sortOrder, isActive = true };
            context.InspectionCategories.Add(category);
            await context.SaveChangesAsync();

            return category;
        }

        public async Task<InspectionCategory> UpdateInspectionCategoryAsync(
            [Service] AppDbContext context,
            int categoryId,
            string? name,
            int? sortOrder)
        {
            var category = await context.InspectionCategories.FindAsync(categoryId);
            if (category == null) throw new GraphQLException("La categoría no existe.");

            if (name != null)
            {
                name = name.Trim();
                if (name.Length == 0) throw new GraphQLException("El nombre de la categoría es obligatorio.");
                if (await context.InspectionCategories.AnyAsync(c => c.name == name && c.categoryId != categoryId))
                    throw new GraphQLException("Ya existe una categoría con ese nombre.");
                category.name = name;
            }
            if (sortOrder.HasValue) category.sortOrder = sortOrder.Value;

            await context.SaveChangesAsync();

            return category;
        }

        public async Task<InspectionCategory> ToggleInspectionCategoryStatusAsync(
            [Service] AppDbContext context,
            int categoryId)
        {
            var category = await context.InspectionCategories.FindAsync(categoryId);
            if (category == null) throw new GraphQLException("La categoría no existe.");

            category.isActive = !category.isActive;
            await context.SaveChangesAsync();

            return category;
        }
    }
}
