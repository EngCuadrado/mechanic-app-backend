using WebApi.Models;
using WebApi.Data;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class ManufacturerMutations
{
    // CREATE
    public async Task<Manufacturer> CreateManufacturerAsync(
        string name, 
        string? phone, 
        string? email, 
        string? website, 
        [Service] AppDbContext context)
    {
        var manufacturer = new Manufacturer 
        { 
            name = name, 
            phone = phone,
            email = email,
            website = website,
            is_active = true 
        };

        context.Manufacturers.Add(manufacturer);
        await context.SaveChangesAsync();
        return manufacturer;
    }

    // UPDATE
    public async Task<Manufacturer> UpdateManufacturerAsync(
        int id, 
        string? name, 
        string? phone,
        string? email,
        string? website,
        bool? is_active, 
        [Service] AppDbContext context)
    {
        var m = await context.Manufacturers.FindAsync(id) 
                ?? throw new GraphQLException(new Error("Manufacturer not found", "NOT_FOUND"));

        if (name != null) m.name = name;
        if (phone != null) m.phone = phone;
        if (email != null) m.email = email;
        if (website != null) m.website = website;
        if (is_active.HasValue) m.is_active = is_active.Value;

        await context.SaveChangesAsync();
        return m;
    }

    // DELETE (Logical)
    public async Task<bool> DeleteManufacturerAsync(int id, [Service] AppDbContext context)
    {
        var m = await context.Manufacturers.FindAsync(id);
        if (m == null) return false;

        m.is_active = false;
        await context.SaveChangesAsync();
        return true;
    }
}
