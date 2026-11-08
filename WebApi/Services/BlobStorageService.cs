using Azure.Storage.Blobs;
using SkiaSharp;


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

            var upload = Optimize(memoryStream) ?? memoryStream;
            upload.Position = 0;
            await blobClient.UploadAsync(upload, overwrite: true);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fallo inesperado al intentar subir el archivo '{blobName}' al contenedor '{containerName}'.");
            throw;
        }
    }

    private const int MaxSide = 2560;
    private const int Quality = 85;

    // Re-encodes JPEG/PNG/WebP (same format, EXIF rotation applied, metadata dropped, capped at MaxSide).
    // Returns null when not worth it (other format, undecodable, or not smaller) so the original is uploaded.
    // shortcut: mirrored EXIF orientations (rare) are not flipped, add a Scale(-1,1) if they show up.
    private MemoryStream? Optimize(MemoryStream original)
    {
        try
        {
            // SKCodec.Create(Stream) cierra el stream al liberarse; con SKData el original sigue abierto para subirlo si no se optimiza.
            using var data = SKData.CreateCopy(original.GetBuffer().AsSpan(0, (int)original.Length));
            using var codec = SKCodec.Create(data);
            if (codec is null || codec.EncodedFormat is not (SKEncodedImageFormat.Jpeg or SKEncodedImageFormat.Png or SKEncodedImageFormat.Webp))
                return null;

            var format = codec.EncodedFormat;
            var origin = codec.EncodedOrigin;
            using var bitmap = SKBitmap.Decode(codec);
            if (bitmap is null) return null;

            var degrees = origin switch
            {
                SKEncodedOrigin.BottomRight => 180f,
                SKEncodedOrigin.RightTop => 90f,
                SKEncodedOrigin.LeftBottom => 270f,
                _ => 0f
            };
            var swap = degrees is 90f or 270f;
            var (w, h) = swap ? (bitmap.Height, bitmap.Width) : (bitmap.Width, bitmap.Height);
            var scale = Math.Min(1f, (float)MaxSide / Math.Max(w, h));
            var info = new SKImageInfo(Math.Max(1, (int)(w * scale)), Math.Max(1, (int)(h * scale)));

            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Translate(info.Width / 2f, info.Height / 2f);
            canvas.RotateDegrees(degrees);
            canvas.Scale(scale);
            canvas.Translate(-bitmap.Width / 2f, -bitmap.Height / 2f);
            using var image = SKImage.FromBitmap(bitmap);
            canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKCubicResampler.Mitchell));

            using var encoded = surface.Snapshot().Encode(format, Quality);
            if (encoded is null || encoded.Size >= original.Length) return null;
            return new MemoryStream(encoded.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo optimizar la imagen; se sube el original.");
            return null;
        }
    }
}
