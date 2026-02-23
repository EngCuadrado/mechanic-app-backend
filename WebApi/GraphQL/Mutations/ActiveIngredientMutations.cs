using WebApi.Data;
using WebApi.Models.Empleados;

namespace WebApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class ActiveIngredientMutations
{
    // CREATE: Agregar un nuevo fármaco/principio activo
    public async Task<ActiveIngredient> CreateActiveIngredientAsync(
        string name, 
        string description, 
        bool is_controlled, 
        [Service] AppDbContext context)
    {
        var activeIngredient = new ActiveIngredient 
        { 
            name = name, 
            description = description, 
            is_controlled = is_controlled, 
            is_active = true 
        };

        context.ActiveIngredients.Add(activeIngredient);
        await context.SaveChangesAsync();
        return activeIngredient;
    }

    // UPDATE: Modificar datos, descripción o estado de control
    public async Task<ActiveIngredient> UpdateActiveIngredientAsync(
        int id, 
        string? name, 
        string? description, 
        bool? is_controlled, 
        bool? is_active, 
        [Service] AppDbContext context)
    {
        var ingredient = await context.ActiveIngredients.FindAsync(id) 
                         ?? throw new GraphQLException(new Error("Active Ingredient not found", "NOT_FOUND"));

        if (name != null) ingredient.name = name;
        if (description != null) ingredient.description = description;
        if (is_controlled.HasValue) ingredient.is_controlled = is_controlled.Value;
        if (is_active.HasValue) ingredient.is_active = is_active.Value;

        await context.SaveChangesAsync();
        return ingredient;
    }

    // DELETE: Eliminación lógica
    public async Task<bool> DeleteActiveIngredientAsync(int id, [Service] AppDbContext context)
    {
        var ingredient = await context.ActiveIngredients.FindAsync(id);
        if (ingredient == null) return false;

        ingredient.is_active = false;
        await context.SaveChangesAsync();
        return true;
    }
}
