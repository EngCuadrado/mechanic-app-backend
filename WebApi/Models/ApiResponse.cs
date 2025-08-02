namespace WebApi.Models
{
    public class ApiResponse
    {
        public Boolean success { get; set; }

        public String message { get; set; }

        public object data { get; set; }

    }
}
