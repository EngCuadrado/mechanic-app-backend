namespace WebApi.Models;

public class UnitOfMeasure
{
    public int unit_of_measure_id { get; set; }
    
    public string name  { get; set; }
    
    public string abbreviation { get; set; }
    
    public bool is_active { get; set; }
}
