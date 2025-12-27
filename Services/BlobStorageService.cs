using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace ExcelOrderSync.Function.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _container;
    private readonly string _defaultFilePath;

    public BlobStorageService(IConfiguration config)
    {
        var conn = config["BlobConnectionString"]
                   ?? throw new InvalidOperationException("Missing BlobConnectionString");

        var containerName = config["BlobContainerName"]
                   ?? throw new InvalidOperationException("Missing BlobContainerName");

        _defaultFilePath = config["ExcelFilePath"]
                   ?? throw new InvalidOperationException("Missing ExcelFilePath");

        _container = new BlobContainerClient(conn, containerName);
        _container.CreateIfNotExists();
    }

    public async Task<Stream> DownloadAsync(string? path = null)
    {
        var blobPath = path ?? _defaultFilePath;
        var blob = _container.GetBlobClient(blobPath);

        if (!await blob.ExistsAsync())
            throw new FileNotFoundException($"Excel not found: {blobPath}");

        var stream = new MemoryStream();
        await blob.DownloadToAsync(stream);
        stream.Position = 0;
        return stream;
    }

    public async Task UploadAsync(string? path, Stream stream)
    {
        var blobPath = path ?? _defaultFilePath;

        stream.Position = 0;
        var blob = _container.GetBlobClient(blobPath);
        await blob.UploadAsync(stream, overwrite: true);
    }
}
