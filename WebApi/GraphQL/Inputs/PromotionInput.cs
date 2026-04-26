using System.Collections.Generic;

namespace WebApi.GraphQL.Inputs
{
    public record CreatePromotionInput(
        string PromotionName,
        string CalculationType, // 'PERCENTAGE' o 'FIXED_AMOUNT'
        decimal DiscountValue,
        DateTime? StartDate,
        DateTime? EndDate,
        bool IsActive,
        bool AppliesToAllProducts,
        List<int>? ProductIds
    );

    public record UpdatePromotionInput(
        int PromotionId,
        string? PromotionName,
        string? CalculationType,
        decimal? DiscountValue,
        DateTime? StartDate,
        DateTime? EndDate,
        bool? IsActive,
        bool? AppliesToAllProducts,
        List<int>? ProductIds
    );
}
