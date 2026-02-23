namespace WebApi.Models;

public class Presentation
{
    public int presentation_id { get; set; }
    
    public string name  { get; set; }
    
    public string description { get; set; }
    
    public bool is_active { get; set; }
}
