using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class    PaymentMethod
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("payment_method_id")]
    public int PaymentMethodId { get; set; }

    // Como no especificaste NOT NULL en el script SQL, 
    // no le agregamos el atributo [Required].
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } // Ej: 'Efectivo', 'Tarjeta de Crédito', 'Transferencia'

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}
