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
            string name,
            string taxId,
            string billingAddress,
            string contactEmail,
            string contactPhone,
            string defaultCurrency,
            string logoUrl)
        {
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
            int companyId,
            string name,
            string taxId,
            string billingAddress,
            string contactEmail,
            string contactPhone,
            string defaultCurrency,
            string logoUrl)
        {
            var company = await context.Companies.FindAsync(companyId);

            if (company == null)
            {
                throw new GraphQLException("La compañía no existe.");
            }

            company.name = name;
            company.taxId = taxId;
            company.billingAddress = billingAddress;
            company.contactEmail = contactEmail;
            company.contactPhone = contactPhone;
            company.defaultCurrency = defaultCurrency;
            company.logoUrl = logoUrl;

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
