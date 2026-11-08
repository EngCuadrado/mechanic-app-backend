using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class WorkOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("work_order_id")]
    public int workOrderId { get; set; }

    [Column("vehicle_id")]
    public int vehicleId { get; set; }

    // Snapshot de la empresa que tenía el vehículo al crear la orden
    [Column("company_id")]
    public int? companyId { get; set; }

    [Column("rental_id")]
    public int? rentalId { get; set; }

    [Column("invoice_id")]
    public int? invoiceId { get; set; }

    // Preventivo, Correctivo, Choque, Entrega, Devolucion
    [Column("type")]
    public string type { get; set; }

    // Recibido, En proceso, Listo, Entregado, Cancelado
    [Column("current_status")]
    public string currentStatus { get; set; }

    [Column("reported_issue")]
    public string? reportedIssue { get; set; }

    [Column("notes")]
    public string? notes { get; set; }

    [Column("created_by")]
    public int? createdBy { get; set; }

    [Column("dropoff_driver_name")]
    public string? dropoffDriverName { get; set; }

    [Column("pickup_driver_name")]
    public string? pickupDriverName { get; set; }

    [Column("dropoff_signature_url")]
    public string? dropoffSignatureUrl { get; set; }

    [Column("pickup_signature_url")]
    public string? pickupSignatureUrl { get; set; }

    [Column("entry_timestamp")]
    public DateTime entryTimestamp { get; set; }

    [Column("exit_timestamp")]
    public DateTime? exitTimestamp { get; set; }

    [Column("total_internal_cost")]
    public decimal? totalInternalCost { get; set; }

    [Column("total_billed_price")]
    public decimal? totalBilledPrice { get; set; }

    [ForeignKey("vehicleId")]
    public Vehicle? Vehicle { get; set; }

    [ForeignKey("companyId")]
    public Company? Company { get; set; }

    [ForeignKey("createdBy")]
    public User? Creator { get; set; }

    [ForeignKey("rentalId")]
    public VehicleRental? Rental { get; set; }

    [ForeignKey("invoiceId")]
    public Invoice? Invoice { get; set; }

    public List<WorkOrderPart> Parts { get; set; } = new();
    public List<WorkOrderService> Services { get; set; } = new();
    public List<WorkOrderPhoto> Photos { get; set; } = new();
    public List<VehicleInspection> Inspections { get; set; } = new();
}

public class WorkOrderPart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("work_order_part_id")]
    public int workOrderPartId { get; set; }

    [Column("work_order_id")]
    public int workOrderId { get; set; }

    [Column("part_id")]
    public int partId { get; set; }

    [Column("quantity")]
    public int quantity { get; set; }

    // Snapshot del costo y precio al usar el repuesto
    [Column("total_unit_cost")]
    public decimal totalUnitCost { get; set; }

    [Column("unit_price")]
    public decimal unitPrice { get; set; }

    [ForeignKey("partId")]
    public InventoryPart? Part { get; set; }
}

public class WorkOrderService
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("work_order_service_id")]
    public int workOrderServiceId { get; set; }

    [Column("work_order_id")]
    public int workOrderId { get; set; }

    [Column("mechanic_id")]
    public int? mechanicId { get; set; }

    [Column("description")]
    public string? description { get; set; }

    [Column("internal_cost")]
    public decimal? internalCost { get; set; }

    [Column("price")]
    public decimal? price { get; set; }

    [ForeignKey("mechanicId")]
    public Mechanics? Mechanic { get; set; }
}

public class WorkOrderPhoto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("work_order_photo_id")]
    public int workOrderPhotoId { get; set; }

    [Column("work_order_id")]
    public int workOrderId { get; set; }

    [Column("vehicle_inspection_id")]
    public int? vehicleInspectionId { get; set; }

    // Antes, Despues, Otro
    [Column("stage")]
    public string stage { get; set; }

    [Column("photo_url")]
    public string photoUrl { get; set; }

    // Zona del vehículo
    [Column("category")]
    public string? category { get; set; }

    [Column("uploaded_at")]
    public DateTime? uploadedAt { get; set; }
}
