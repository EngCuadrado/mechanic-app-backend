using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

// Libro de stock: inmutable (la BD rechaza UPDATE/DELETE). Un trigger actualiza inventory_parts.stock_quantity.
public class InventoryMovement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("movement_id")]
    public int movementId { get; set; }

    [Column("part_id")]
    public int partId { get; set; }

    // Con signo: Inicial/Compra/Devolucion > 0, Uso < 0, Ajuste <> 0
    [Column("quantity")]
    public int quantity { get; set; }

    // Inicial, Compra, Uso, Devolucion, Ajuste
    [Column("movement_type")]
    public string movementType { get; set; }

    [Column("unit_cost")]
    public decimal? unitCost { get; set; }

    [Column("work_order_id")]
    public int? workOrderId { get; set; }

    [Column("notes")]
    public string? notes { get; set; }

    [Column("created_by")]
    public int? createdBy { get; set; }

    [Column("created_at")]
    public DateTime? createdAt { get; set; }

    [ForeignKey("partId")]
    public InventoryPart? Part { get; set; }

    [ForeignKey("createdBy")]
    public User? CreatedByUser { get; set; }
}
