using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class PromotionProduct
{
    [Column("promotion_id")]
    public int PromotionId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    // --- Propiedades de Navegación ---
    
    [ForeignKey(nameof(PromotionId))]
    public virtual Promotion Promotion { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; }
}
