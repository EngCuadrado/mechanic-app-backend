using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class MechanicMutations
    {
        public async Task<Mechanics> AddMechanicAsync(
            [Service] AppDbContext context,
            string firstName,
            string lastName,
            int? specialtyId)
        {
            MechanicSpecialties specialtyExists = null;

            // Opcional: Validar que la especialidad exista si specialtyId tiene valor
            if (specialtyId.HasValue)
            {
                specialtyExists = await context.mechanicSpecialties.FindAsync(specialtyId.Value);
                if (specialtyExists == null)
                {
                    throw new GraphQLException("La especialidad asignada no existe.");
                }
            }

            var newMechanic = new Mechanics
            {
                FirstName = firstName,
                LastName = lastName,
                SpecialtyId = specialtyId,
                IsActive = true,
                Specialty = specialtyExists
            };

            context.Mechanics.Add(newMechanic);
            await context.SaveChangesAsync();

            return newMechanic;
        }

        public async Task<Mechanics> UpdateMechanicAsync(
            [Service] AppDbContext context,
            int mechanicId,
            string firstName,
            string lastName,
            int? specialtyId)
        {
            var mechanic = await context.Mechanics
                .Include(m => m.Specialty)
                .FirstOrDefaultAsync(m => m.MechanicId == mechanicId);

            if (mechanic == null)
            {
                throw new GraphQLException("El mecánico no existe.");
            }

            if (specialtyId.HasValue && mechanic.SpecialtyId != specialtyId.Value)
            {
                var specialtyExists = await context.mechanicSpecialties.FindAsync(specialtyId.Value);
                if (specialtyExists == null)
                {
                    throw new GraphQLException("La especialidad asignada no existe.");
                }
                mechanic.Specialty = specialtyExists;
            }
            else if (!specialtyId.HasValue)
            {
                mechanic.Specialty = null;
            }

            mechanic.FirstName = firstName;
            mechanic.LastName = lastName;
            mechanic.SpecialtyId = specialtyId;

            await context.SaveChangesAsync();

            return mechanic;
        }

        public async Task<Mechanics> ToggleMechanicStatusAsync(
            [Service] AppDbContext context,
            int mechanicId)
        {
            var mechanic = await context.Mechanics.FindAsync(mechanicId);

            if (mechanic == null)
            {
                throw new GraphQLException("El mecánico no existe.");
            }

            // Cambiamos el estado de activo a inactivo o viceversa
            mechanic.IsActive = !mechanic.IsActive;

            await context.SaveChangesAsync();

            return mechanic;
        }
    }
}
