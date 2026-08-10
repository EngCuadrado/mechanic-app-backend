using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Mechanics
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("mechanic_id")]
    public int MechanicId { get; set; }

    [MaxLength(255)]
    [Column("first_name")]
    public string FirstName { get; set; }

    [MaxLength(255)]
    [Column("last_name")]
    public string LastName { get; set; }

    [Column("specialty_id")]
    public int? SpecialtyId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public virtual MechanicSpecialties Specialty { get; set; }
}