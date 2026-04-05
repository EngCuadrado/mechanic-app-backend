using System;
using System.Collections.Generic;

namespace WebApi.GraphQL.Inputs
{
    public record MedicineActiveIngredientInput(
        int ActiveIngredientId,
        decimal DoseValue,
        int DoseUnitId
    );

    public record AddMedicineInput(
        string Name,
        int IdBrand,
        int ManufacturerId,
        int CategoryId,
        int AdministrationRouteId,
        bool RequiresPrescription,
        int SupplierId,
        int PresentationId,
        int UnitOfMeasureId,
        int UnitsPerPresentation,
        string Currency,
        decimal PricePerUnit,
        decimal PriceFullPresentation,
        bool IsFractionable,
        string Description,
        string BatchCode,
        DateTime ExpirationDate,
        int Units,
        int StockUnits,
        int MinStockUnits,
        List<MedicineActiveIngredientInput> Ingredients
    );
}
