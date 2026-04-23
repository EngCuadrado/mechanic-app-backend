using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class SaleDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("sale_detail_id")]
    public int SaleDetailId { get; set; }

    [Required]
    [Column("sale_id")]
    public int SaleId { get; set; }

    [Required]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Required]
    [Column("batch_id")]
    public int BatchId { get; set; } // Obligatorio para trazabilidad (qué lote se entregó)

    [Column("promotion_id")]
    public int? PromotionId { get; set; } // Nullable (?) si no aplicó descuento

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; } // Siempre en la unidad mínima

    [Column("is_full_presentation")]
    public bool IsFullPresentation { get; set; } = false; // BIT DEFAULT 0

    // --- "Snapshot" (Copia estática al momento de la venta) ---

    [Required]
    [Column("applied_cost_price", TypeName = "decimal(18,4)")]
    public decimal AppliedCostPrice { get; set; }

    [Required]
    [Column("applied_unit_price", TypeName = "decimal(18,4)")]
    public decimal AppliedUnitPrice { get; set; }

    [Required]
    [Column("applied_tax_rate", TypeName = "decimal(5,4)")]
    public decimal AppliedTaxRate { get; set; } // Ej: 0.1500 (15%)

    // --- Cálculos de la línea ---

    [Column("calculated_discount", TypeName = "decimal(18,2)")]
    public decimal CalculatedDiscount { get; set; } = 0m;

    [Required]
    [Column("calculated_tax", TypeName = "decimal(18,2)")]
    public decimal CalculatedTax { get; set; }

    [Required]
    [Column("line_total", TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; } // (Precio * Cantidad) - Descuento + Impuesto

    // --- Propiedades de Navegación (Claves Foráneas) ---

    [ForeignKey(nameof(SaleId))]
    public virtual Sale Sale { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; }

    [ForeignKey(nameof(BatchId))]
    public virtual Batch Batch { get; set; }

    [ForeignKey(nameof(PromotionId))]
    public virtual Promotion Promotion { get; set; }
}
