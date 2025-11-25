using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace PhotoMemories.Api.Services;

public interface IBlobStorageService
{
    Task<string> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string contentType);
    Task<string> UploadVideoAsync(Guid userId, Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string blobUrl);
    Task DeleteFileAsync(string blobUrl);
    string GenerateSasUrl(string blobUrl, TimeSpan validFor);
    Task<string> GenerateUploadSasUrlAsync(Guid userId, string fileName, string contentType, bool isVideo);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _photosContainerName = "photos";
    private readonly string _videosContainerName = "videos";

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_photosContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobName = $"{userId}/{Guid.NewGuid()}/{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(fileStream, options);
        return blobClient.Uri.ToString();
    }

    public async Task<string> UploadVideoAsync(Guid userId, Stream fileStream, string fileName, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_videosContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobName = $"{userId}/{Guid.NewGuid()}/{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(fileStream, options);
        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        var containerName = uri.Segments[1].TrimEnd('/');
        var blobName = string.Join("", uri.Segments.Skip(2));

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync();
        return response.Value.Content;
    }

    public async Task DeleteFileAsync(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        var containerName = uri.Segments[1].TrimEnd('/');
        var blobName = string.Join("", uri.Segments.Skip(2));

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }

    public string GenerateSasUrl(string blobUrl, TimeSpan validFor)
    {
        var uri = new Uri(blobUrl);
        var containerName = uri.Segments[1].TrimEnd('/');
        var blobName = string.Join("", uri.Segments.Skip(2));

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (blobClient.CanGenerateSasUri)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        return blobUrl;
    }

    public async Task<string> GenerateUploadSasUrlAsync(Guid userId, string fileName, string contentType, bool isVideo)
    {
        var containerName = isVideo ? _videosContainerName : _photosContainerName;
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobName = $"{userId}/{Guid.NewGuid()}/{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        if (blobClient.CanGenerateSasUri)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        throw new InvalidOperationException("Cannot generate SAS URL");
    }
}
