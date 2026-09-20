using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class BrandMutations
    {
        public async Task<Brand> AddBrandAsync(
            [Service] AppDbContext context, 
            string brandName)
        {
            var newBrand = new Brand
            {
                brandName = brandName
            };

            context.Brands.Add(newBrand);
            await context.SaveChangesAsync();

            return newBrand;
        }
   
        public async Task<Brand> UpdateBrandAsync(
            [Service] AppDbContext context,
            int brandId,
            string newBrandName)
        {
            var brand = await context.Brands.FindAsync(brandId);

            if (brand == null)
            {
                throw new GraphQLException("La marca no existe.");
            }

            brand.brandName = newBrandName;

            await context.SaveChangesAsync();

            return brand;
        }

        public async Task<bool> DeleteBrandAsync(
            [Service] AppDbContext context,
            int brandId)
        {
            var brand = await context.Brands.FindAsync(brandId);

            if (brand == null)
            {
                throw new GraphQLException("La marca no existe.");
            }

            context.Brands.Remove(brand);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
