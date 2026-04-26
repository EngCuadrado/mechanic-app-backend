namespace WebApi.GraphQL.Payloads
{
    public record SalePayload(
        int? SaleId,
        string? ReceiptNumber,
        bool Success,
        string Message
    );
}
