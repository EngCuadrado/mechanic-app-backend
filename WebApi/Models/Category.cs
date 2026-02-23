using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Category
{
    public int category_id { get; set; }
   

    public string? name { get; set; } 
   

    public string? description { get; set; }
    
    public bool is_active { get; set; }
}
