using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class VehicleInspection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("vehicle_inspection_id")]
    public int vehicleInspectionId { get; set; }

    [Column("work_order_id")]
    public int workOrderId { get; set; }

    // Entrada, Salida
    [Column("inspection_type")]
    public string inspectionType { get; set; }

    [Column("mileage")]
    public int? mileage { get; set; }

    [Column("inspection_notes")]
    public string? inspectionNotes { get; set; }

    [Column("damage_map_url")]
    public string? damageMapUrl { get; set; }

    [Column("created_at")]
    public DateTime? createdAt { get; set; }

    public List<VehicleInspectionResult> Results { get; set; } = new();
}

public class VehicleInspectionResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("result_id")]
    public int resultId { get; set; }

    [Column("vehicle_inspection_id")]
    public int vehicleInspectionId { get; set; }

    [Column("item_id")]
    public int itemId { get; set; }

    [Column("value")]
    public string? value { get; set; }

    [Column("notes")]
    public string? notes { get; set; }

    [ForeignKey("itemId")]
    public InspectionChecklistItem? Item { get; set; }
}
