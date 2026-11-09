using WebApi.Models;

namespace WebApi.GraphQL.Payloads
{
    // Formulario de inspección: checklist agrupado por categoría (orden sort_order) con el valor guardado.
    public record InspectionFormItem(InspectionChecklistItem Item, string? Value, string? Notes);
    public record InspectionFormGroup(InspectionCategory? Category, List<InspectionFormItem> Items);
    public record InspectionForm(VehicleInspection? Inspection, List<InspectionFormGroup> Groups);

    public record PhotoStageGroup(string Stage, List<WorkOrderPhoto> Photos);

    // Filas de la vista inventory_stock_check (columnas genéricas: la vista debe estar vacía).
    public record StockCheckCell(string Name, string? Value);
    public record StockCheckRow(List<StockCheckCell> Cells);
    public record StockCheck(bool IsConsistent, List<StockCheckRow> Rows);
}

public record WorkOrderStatusCount(string Status, int Count);

public record WorkOrderDashboard(int CreatedToday, int ReadyToDeliver, int Stalled);
