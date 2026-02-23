namespace WebApi.Models;

public class SupplierType
{
    public int supplier_type_id { get; set; }
    public string type_name { get; set; }
    public string description { get; set; }
    
    public ICollection<Supplier> Suppliers { get; set; }

}
