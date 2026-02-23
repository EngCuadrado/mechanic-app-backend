using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Batch
{
    [Key]
    public int batch_id { get; set; }

    public int product_id { get; set; }

    [Required]
    [MaxLength(50)]
    public string batch_code { get; set; }

    public DateTime expiration_date { get; set; }

    public int initial_quantity_units { get; set; }

    public int current_quantity_units { get; set; }

    public bool is_active { get; set; } = true;

    public DateTime created_at { get; set; } = DateTime.Now;

    // --- Propiedad de Navegación ---
    
    [ForeignKey("product_id")]
    public virtual Product product { get; set; }
}
