using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class MechanicSpecialties
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("specialty_id")]
    public int SpecialtyId { get; set; }

    [MaxLength(255)]
    public required string Name { get; set; }

    public ICollection<Mechanics> Mechanics { get; set; } = new List<Mechanics>();
}

/*
 CREATE TABLE [mechanic_specialties] (
  [specialty_id] INT IDENTITY(1,1) PRIMARY KEY,
  [name] nvarchar(255),
  -- [description] text
)
GO
*/
