namespace WebApi.GraphQL.Mutations;

using WebApi.Models;
using WebApi.Data; // Tu DbContext

[ExtendObjectType("Mutation")]
public class CategoryMutations
{
    public async Task<Category> CreateCategoryAsync(
        string name, string description, [Service] AppDbContext context)
    {
        var category = new Category { name = name, description = description, is_active = true };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(
        int id, string? name, string? description, bool? is_active, [Service] AppDbContext context)
    {
        var category = await context.Categories.FindAsync(id) ?? throw new GraphQLException("Not found");
        if (name != null) category.name = name;
        if (description != null) category.description = description;
        if (is_active.HasValue) category.is_active = is_active.Value;
        
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id, [Service] AppDbContext context)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null) return false;
        category.is_active = false; // Eliminación lógica
        await context.SaveChangesAsync();
        return true;
    }
}
