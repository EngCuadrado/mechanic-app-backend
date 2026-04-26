using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Promotion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("promotion_id")]
    public int PromotionId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("promotion_name")]
    public string PromotionName { get; set; } // Ej: 'Tercera Edad', 'Bono Navideño'

    [Required]
    [MaxLength(20)]
    [Column("calculation_type")]
    public string CalculationType { get; set; } // Valores: 'PERCENTAGE' o 'FIXED_AMOUNT'

    [Required]
    [Column("discount_value", TypeName = "decimal(18,4)")]
    public decimal DiscountValue { get; set; } // Precisión de 4 decimales (Ej: 0.2000 o 50.0000)

    // Al no tener NOT NULL ni un DEFAULT en SQL, en C# deben ser DateTime? (Nullable)
    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true; // BIT DEFAULT 1 se traduce a bool = true

    [Column("applies_to_all_products")]
    public bool AppliesToAllProducts { get; set; } = false; // BIT DEFAULT 0 se traduce a bool = false

    // --- Propiedades de Navegación Inversa ---

    // Descomenta esta línea cuando crees la clase PromotionProduct. 
    // Es la relación 1 a N hacia la tabla intermedia que asocia promociones con productos específicos.
     public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
}
