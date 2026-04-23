using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Required]
    [Column("batch_id")]
    public int BatchId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("transaction_type")]
    public string TransactionType { get; set; } // 'SALE', 'PURCHASE', 'ADJUSTMENT', etc.

    [Required]
    [Column("quantity_moved")]
    public int QuantityMoved { get; set; } // Negativo para ventas, positivo para compras

    [Required]
    [Column("stock_after_transaction")]
    public int StockAfterTransaction { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // --- Propiedades de Navegación ---

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; }

    [ForeignKey(nameof(BatchId))]
    public virtual Batch Batch { get; set; }
}
