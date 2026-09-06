using Azure.Storage.Blobs;


public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobStorageService> _logger;


    public BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public async Task<string> UploadImageAsync(MemoryStream memoryStream, string blobName, string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            memoryStream.Position = 0;
            await blobClient.UploadAsync(memoryStream, overwrite: true);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fallo inesperado al intentar subir el archivo '{blobName}' al contenedor '{containerName}'.");
            throw;
        }
    }
}
