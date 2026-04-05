namespace WebApi.Models
{
    public class Brands
    {
        public int id_brand { get; set; }

        public string name { get; set; }
        
        public string? logo_url { get; set; }
        
        public string? contact_phone { get; set; }
        
        public string? contact_email { get; set; }
        
        public DateTime? created_at { get; set; }
        
        public DateTime? updated_at { get; set; }
        public bool is_active { get; set; }
    }
}
