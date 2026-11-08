using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class InspectionCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("category_id")]
    public int categoryId { get; set; }

    [Column("name")]
    public string name { get; set; }

    [Column("sort_order")]
    public int sortOrder { get; set; }

    [Column("is_active")]
    public bool isActive { get; set; } = true;
}
