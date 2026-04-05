using WebApi.Models;
using WebApi.Data;

namespace WebApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class PresentationMutations
{
    public async Task<Presentation> CreatePresentationAsync(
        string name, string description, [Service] AppDbContext context)
    {
        var presentation = new Presentation { name = name, description = description, is_active = true };
        context.Presentations.Add(presentation);
        await context.SaveChangesAsync();
        return presentation;
    }

    public async Task<Presentation> UpdatePresentationAsync(
        int id, string? name, string? description, bool? is_active, [Service] AppDbContext context)
    {
        var p = await context.Presentations.FindAsync(id) ?? throw new GraphQLException("Not found");
        if (name != null) p.name = name;
        if (description != null) p.description = description;
        if (is_active.HasValue) p.is_active = is_active.Value;

        await context.SaveChangesAsync();
        return p;
    }

    public async Task<bool> DeletePresentationAsync(int id, [Service] AppDbContext context)
    {
        var p = await context.Presentations.FindAsync(id);
        if (p == null) return false;
        p.is_active = false;
        await context.SaveChangesAsync();
        return true;
    }
}
