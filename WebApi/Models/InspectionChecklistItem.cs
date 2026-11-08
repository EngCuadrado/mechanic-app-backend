using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class InspectionChecklistItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("item_id")]
    public int itemId { get; set; }

    [Column("name")]
    public string name { get; set; }

    // text, textarea, number, checkbox, date, select, radio
    [Column("input_type")]
    public string inputType { get; set; }

    // En BD es un string JSON (["a","b"]); el value converter de AppDbContext lo expone como lista.
    [Column("options")]
    public List<string>? options { get; set; }

    [Column("category_id")]
    public int? categoryId { get; set; }

    [Column("is_required")]
    public bool isRequired { get; set; } = true;

    [Column("is_active")]
    public bool isActive { get; set; } = true;

    [ForeignKey("categoryId")]
    public InspectionCategory? Category { get; set; }
}
