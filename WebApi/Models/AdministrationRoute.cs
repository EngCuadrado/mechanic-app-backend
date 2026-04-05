namespace WebApi.Models;

public class AdministrationRoute
{
    public int administration_route_id { get; set; }
    
    public string name { get; set; }
    
    public string description { get; set; }
    
    public bool is_active { get; set; }
}
