using System;

namespace WebApi.GraphQL.Inputs
{
    public record AddBatchInput(
        int ProductId,
        string BatchCode,
        DateTime ExpirationDate,
        int QuantityUnits
    );
}
