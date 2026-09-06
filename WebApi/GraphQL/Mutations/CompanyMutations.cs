using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class CompanyMutations
    {
        public async Task<Company> AddCompanyAsync(
            [Service] AppDbContext context,
            [Service] BlobStorageService blobStorageService,
            string name,
            string taxId,
            string billingAddress,
            string contactEmail,
            string contactPhone,
            string defaultCurrency,
            IFile? logoFile)
        {
            string logoUrl = "";

            if (logoFile != null)
            {
                var uid = Guid.NewGuid().ToString();
                var blobName = $"logo/{uid}.JPEG";

                using var memoryStream = new MemoryStream();
                await logoFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Subir al blob storage, asumiendo el contenedor "photos" como en otros endpoints
                logoUrl = await blobStorageService.UploadImageAsync(memoryStream, blobName, "photos");
            }

            var newCompany = new Company
            {
                name = name,
                taxId = taxId,
                billingAddress = billingAddress,
                contactEmail = contactEmail,
                contactPhone = contactPhone,
                defaultCurrency = string.IsNullOrWhiteSpace(defaultCurrency) ? "NIO" : defaultCurrency,
                logoUrl = logoUrl,
                isActive = true
            };

            context.Companies.Add(newCompany);
            await context.SaveChangesAsync();

            return newCompany;
        }

        public async Task<Company> UpdateCompanyAsync(
            [Service] AppDbContext context,
            [Service] BlobStorageService blobStorageService,
            int companyId,
            string name,
            string taxId,
            string billingAddress,
            string contactEmail,
            string contactPhone,
            string defaultCurrency,
            string? logoUrl,
            IFile? logoFile)
        {
            var company = await context.Companies.FindAsync(companyId);

            if (company == null)
            {
                throw new GraphQLException("La compañía no existe.");
            }
            
            string finalLogoUrl = company.logoUrl;

            if (logoFile != null)
            {
                var uid = Guid.NewGuid().ToString();
                var blobName = $"logo/{uid}.JPEG";

                using var memoryStream = new MemoryStream();
                await logoFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                finalLogoUrl = await blobStorageService.UploadImageAsync(memoryStream, blobName, "photos");
            }
            else if (logoUrl != null)
            {
                finalLogoUrl = logoUrl;
            }

            company.name = name;
            company.taxId = taxId;
            company.billingAddress = billingAddress;
            company.contactEmail = contactEmail;
            company.contactPhone = contactPhone;
            company.defaultCurrency = defaultCurrency;
            company.logoUrl = finalLogoUrl;

            await context.SaveChangesAsync();

            return company;
        }

        public async Task<Company> ToggleCompanyStatusAsync(
            [Service] AppDbContext context,
            int companyId)
        {
            var company = await context.Companies.FindAsync(companyId);

            if (company == null)
            {
                throw new GraphQLException("La compañía no existe.");
            }

            // Cambiamos el estado de activo a inactivo o viceversa
            company.isActive = !company.isActive;

            await context.SaveChangesAsync();

            return company;
        }
    }
}
