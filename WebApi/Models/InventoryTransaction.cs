using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Models.Empleados;

namespace WebApi.Models;

public class InventoryTransaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [Required]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("batch_id")]
    public int? BatchId { get; set; }

    [Required]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("transaction_type")]
    public string TransactionType { get; set; } // 'SALE', 'PURCHASE', 'ADJUSTMENT', etc.

    [Required]
    [Column("quantity_moved")]
    public int QuantityMoved { get; set; } // Negativo para ventas, positivo para compras

    [Required]
    [Column("stock_after_transaction")]
    public int StockAfterTransaction { get; set; }

    [Required]
    [Column("unit_cost", TypeName = "decimal(18,4)")]
    public decimal UnitCost { get; set; }

    [MaxLength(50)]
    [Column("reference_document_type")]
    public string ReferenceDocumentType { get; set; }

    [Column("reference_document_id")]
    public int? ReferenceDocumentId { get; set; }

    [MaxLength(255)]
    [Column("notes")]
    public string Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // --- Propiedades de Navegación ---

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; }

    [ForeignKey(nameof(BatchId))]
    public virtual Batch Batch { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee Employee { get; set; }
}
