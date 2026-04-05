using WebApi.Models;
using WebApi.Data;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class AdministrationRouteMutations
{
    // CREATE: Agregar una nueva vía (Ej: Oral, Intravenosa)
    public async Task<AdministrationRoute> CreateAdministrationRouteAsync(
        string name, 
        string description, 
        [Service] AppDbContext context)
    {
        var route = new AdministrationRoute 
        { 
            name = name, 
            description = description, 
            is_active = true 
        };

        context.AdministrationRoutes.Add(route);
        await context.SaveChangesAsync();
        return route;
    }

    // UPDATE: Modificar nombre, descripción o estado
    public async Task<AdministrationRoute> UpdateAdministrationRouteAsync(
        int id, 
        string? name, 
        string? description, 
        bool? is_active, 
        [Service] AppDbContext context)
    {
        var route = await context.AdministrationRoutes.FindAsync(id) 
                    ?? throw new GraphQLException(new Error("Administration Route not found", "NOT_FOUND"));

        if (name != null) route.name = name;
        if (description != null) route.description = description;
        if (is_active.HasValue) route.is_active = is_active.Value;

        await context.SaveChangesAsync();
        return route;
    }

    // DELETE: Eliminación lógica (Desactivar vía)
    public async Task<bool> DeleteAdministrationRouteAsync(int id, [Service] AppDbContext context)
    {
        var route = await context.AdministrationRoutes.FindAsync(id);
        if (route == null) return false;

        route.is_active = false;
        await context.SaveChangesAsync();
        return true;
    }
}
