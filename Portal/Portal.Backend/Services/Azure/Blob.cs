using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Portal.Backend.Services.Azure;

public class Blob
{
    public async Task UploadBlobAsync(string connectionString, string containerName, string blobName, Stream content)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: true);
    }

    // connection string = from environment variables
    // containerName = authors
    // blobName = {authorId}/{assetName}/{version}/{assetName}_{version}.zip
    public async Task<string> DownloadBlobAsync(string connectionString, string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var download = await blobClient.DownloadContentAsync();
        return download.Value.Content.ToString();
    }
}
