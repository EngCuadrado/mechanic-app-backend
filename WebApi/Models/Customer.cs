using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [MaxLength(50)]
    [Column("identification_type")]
    public string IdentificationType { get; set; }

    [MaxLength(50)]
    [Column("identification_number")]
    public string IdentificationNumber { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("first_name")]
    public string FirstName { get; set; }

    [MaxLength(100)]
    [Column("last_name")]
    public string LastName { get; set; }

    [MaxLength(255)]
    [Column("address")]
    public string Address { get; set; }

    [MaxLength(20)]
    [Column("phone")]
    public string Phone { get; set; }

    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    // --- Propiedad de Navegación Inversa ---

    // Relación 1 a N: Un cliente puede tener múltiples ventas asociadas.
    // Se inicializa la lista para evitar excepciones de referencia nula (NullReferenceException).
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
