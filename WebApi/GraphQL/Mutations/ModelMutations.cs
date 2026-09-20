using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class ModelMutations
    {
        public async Task<Model> AddModelAsync(
            [Service] AppDbContext context, 
            int brandId,
            string modelName)
        {
            var brandExists = await context.Brands.FindAsync(brandId);
            if (brandExists == null)
            {
                throw new GraphQLException("La marca especificada no existe.");
            }

            var newModel = new Model
            {
                brandId = brandId,
                modelName = modelName
            };

            context.Models.Add(newModel);
            await context.SaveChangesAsync();

            return newModel;
        }
   
        public async Task<Model> UpdateModelAsync(
            [Service] AppDbContext context,
            int modelId,
            int? newBrandId,
            string? newModelName)
        {
            var model = await context.Models.FindAsync(modelId);

            if (model == null)
            {
                throw new GraphQLException("El modelo no existe.");
            }

            if (newBrandId.HasValue)
            {
                var brandExists = await context.Brands.FindAsync(newBrandId.Value);
                if (brandExists == null)
                {
                    throw new GraphQLException("La marca especificada no existe.");
                }
                model.brandId = newBrandId.Value;
            }

            if (!string.IsNullOrEmpty(newModelName))
            {
                model.modelName = newModelName;
            }

            await context.SaveChangesAsync();

            return model;
        }

        public async Task<bool> DeleteModelAsync(
            [Service] AppDbContext context,
            int modelId)
        {
            var model = await context.Models.FindAsync(modelId);

            if (model == null)
            {
                throw new GraphQLException("El modelo no existe.");
            }

            context.Models.Remove(model);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
