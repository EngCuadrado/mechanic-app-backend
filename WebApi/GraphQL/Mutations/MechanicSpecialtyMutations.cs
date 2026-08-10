using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class MechanicSpecialtyMutations
    {
        public async Task<MechanicSpecialties> AddMechanicSpecialtyAsync(
            [Service] AppDbContext context, 
            string name)
        {
            var newSpecialty = new MechanicSpecialties
            {
                Name = name
            };

            context.mechanicSpecialties.Add(newSpecialty);
            await context.SaveChangesAsync();

            return newSpecialty;
        }
   
        public async Task<MechanicSpecialties> UpdateMechanicSpecialtyAsync(
            [Service] AppDbContext context,
            int specialtyId,
            string newName)
        {
            var specialty = await context.mechanicSpecialties.FindAsync(specialtyId);

            if (specialty == null)
            {
                throw new GraphQLException("La especialidad no existe.");
            }

            specialty.Name = newName;

            await context.SaveChangesAsync();

            return specialty;
        }

        public async Task<bool> DeleteMechanicSpecialtyAsync(
            [Service] AppDbContext context,
            int specialtyId)
        {
            var specialty = await context.mechanicSpecialties.FindAsync(specialtyId);

            if (specialty == null)
            {
                throw new GraphQLException("La especialidad no existe.");
            }

            context.mechanicSpecialties.Remove(specialty);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
