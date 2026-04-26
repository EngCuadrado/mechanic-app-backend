using System.Collections.Generic;

namespace WebApi.GraphQL.Inputs
{
    public record CreateSaleInput(
        int EmployeeId,
        int? CustomerId,
        string ReceiptType,
        string ReceiptNumber,
        string? PrescriptionNumber,
        string? DoctorName,
        string? Currency,
        List<SaleDetailInput> Details,
        List<SalePaymentInput> Payments
    );

    public record SaleDetailInput(
        int ProductId,
        int BatchId,
        int Quantity,
        int? PromotionId,
        bool IsFullPresentation
    );

    public record SalePaymentInput(
        int PaymentMethodId,
        decimal Amount,
        string? TransactionReference
    );
}
