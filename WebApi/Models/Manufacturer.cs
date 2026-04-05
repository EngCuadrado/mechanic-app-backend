namespace WebApi.Models;

public class Manufacturer
{
    public int manufacturer_id { get; set; }
    
    public string name { get; set; }
    
    public string phone { get; set; }
    
    public string email { get; set; }
    
    public string website { get; set; }
    
    public bool is_active { get; set; }
}
