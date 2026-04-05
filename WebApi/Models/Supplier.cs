namespace WebApi.Models;

public class Supplier
{
    public int supplier_id { get; set; }
    public int supplier_type_id { get; set; }
    public string? company_name { get; set; }
    public string? tax_id { get; set; }
    public string? contact_name { get; set; }
    public string? phone { get; set; }
    public string? address { get; set; }
    public string? email { get; set; }
    public string? website { get; set; }
    public bool is_active { get; set; }
    
    public SupplierType type { get; set; }
}
