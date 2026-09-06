namespace WebApi.Models;


public class BlobRequestData
{
    public string blobName { get; set; }
    public IFormFile Picture { get; set; }
}
