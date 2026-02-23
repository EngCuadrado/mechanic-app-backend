using WebApi.Models;
using WebApi.Data;

namespace WebApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class UnitOfMeasureMutations
{
    public async Task<UnitOfMeasure> CreateUnitOfMeasureAsync(
        string name, string abbreviation, [Service] AppDbContext context)
    {
        var unit = new UnitOfMeasure { name = name, abbreviation = abbreviation, is_active = true };
        context.UnitOfMeasures.Add(unit);
        await context.SaveChangesAsync();
        return unit;
    }

    public async Task<UnitOfMeasure> UpdateUnitOfMeasureAsync(
        int id, string? name, string? abbreviation, bool? is_active, [Service] AppDbContext context)
    {
        var u = await context.UnitOfMeasures.FindAsync(id) ?? throw new GraphQLException("Not found");
        if (name != null) u.name = name;
        if (abbreviation != null) u.abbreviation = abbreviation;
        if (is_active.HasValue) u.is_active = is_active.Value;

        await context.SaveChangesAsync();
        return u;
    }

    public async Task<bool> DeleteUnitOfMeasureAsync(int id, [Service] AppDbContext context)
    {
        var u = await context.UnitOfMeasures.FindAsync(id);
        if (u == null) return false;
        u.is_active = false;
        await context.SaveChangesAsync();
        return true;
    }
}
