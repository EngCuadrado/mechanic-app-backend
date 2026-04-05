namespace WebApi.Models.Empleados;

public class ActiveIngredient
{
    public int active_ingredient_id { get; set; }
    
    public string name { get; set; }
    
    public string description { get; set; }
    
    public bool is_controlled { get; set; }
    
    public bool is_active { get; set; }
}
