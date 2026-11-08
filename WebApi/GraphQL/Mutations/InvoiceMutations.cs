using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InvoiceMutations
    {
        public async Task<Invoice> AddInvoiceAsync(
            [Service] AppDbContext context,
            int companyId,
            string invoiceNumber,
            string status,
            DateTime? issueDate,
            DateTime? dueDate,
            decimal subtotal,
            decimal taxRate,
            decimal taxAmount,
            decimal totalAmount,
            string? notes,
            string currency = "USD",
            decimal exchangeRate = 1)
        {
            invoiceNumber = invoiceNumber?.Trim() ?? "";
            if (invoiceNumber.Length == 0) throw new GraphQLException("El número de factura es obligatorio.");
            if (string.IsNullOrWhiteSpace(status)) throw new GraphQLException("El estado es obligatorio.");
            if (!await context.Companies.AnyAsync(c => c.companyId == companyId)) throw new GraphQLException("La empresa no existe.");
            if (await context.Invoices.AnyAsync(i => i.invoiceNumber == invoiceNumber))
                throw new GraphQLException("Ya existe una factura con ese número.");

            var invoice = new Invoice
            {
                companyId = companyId, invoiceNumber = invoiceNumber, status = status,
                issueDate = issueDate, dueDate = dueDate, subtotal = subtotal, taxRate = taxRate,
                taxAmount = taxAmount, totalAmount = totalAmount, currency = currency,
                exchangeRate = exchangeRate, notes = notes, createdAt = DateTime.Now
            };
            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice> UpdateInvoiceAsync(
            [Service] AppDbContext context,
            int id,
            string? invoiceNumber,
            string? status,
            Optional<DateTime?> issueDate,
            Optional<DateTime?> dueDate,
            decimal? subtotal,
            decimal? taxRate,
            decimal? taxAmount,
            decimal? totalAmount,
            string? currency,
            decimal? exchangeRate,
            Optional<string?> notes)
        {
            var invoice = await context.Invoices.FindAsync(id) ?? throw new GraphQLException("La factura no existe.");

            if (invoiceNumber != null)
            {
                invoiceNumber = invoiceNumber.Trim();
                if (invoiceNumber.Length == 0) throw new GraphQLException("El número de factura es obligatorio.");
                if (await context.Invoices.AnyAsync(i => i.invoiceNumber == invoiceNumber && i.id != id))
                    throw new GraphQLException("Ya existe una factura con ese número.");
                invoice.invoiceNumber = invoiceNumber;
            }
            if (status != null) invoice.status = status;
            if (issueDate.HasValue) invoice.issueDate = issueDate.Value;
            if (dueDate.HasValue) invoice.dueDate = dueDate.Value;
            if (subtotal.HasValue) invoice.subtotal = subtotal;
            if (taxRate.HasValue) invoice.taxRate = taxRate;
            if (taxAmount.HasValue) invoice.taxAmount = taxAmount;
            if (totalAmount.HasValue) invoice.totalAmount = totalAmount;
            if (currency != null) invoice.currency = currency;
            if (exchangeRate.HasValue) invoice.exchangeRate = exchangeRate;
            if (notes.HasValue) invoice.notes = notes.Value;

            await context.SaveChangesAsync();
            return invoice;
        }
    }
}
