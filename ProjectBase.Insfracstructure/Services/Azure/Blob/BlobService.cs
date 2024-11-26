using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using ProjectBase.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Services.Azure.Blob
{
    [ExcludeFromCodeCoverage]
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _service;
        private BlobContainerClient? _containerClient;
        private readonly string _containerName = "coffee-data";
        public BlobService(BlobServiceClient service)
        {
            _service = service;
        }
        private async Task GetContainerClient()
        {
            _containerClient = _service.GetBlobContainerClient(_containerName);
            await _containerClient.CreateIfNotExistsAsync();
        }
        public async Task UploadFile(IFormFile file)
        {
            await GetContainerClient();

            var blobClient = _containerClient?.GetBlobClient(file.FileName);
            if (blobClient is null)
            {
                throw new NullException("Blob client not found");
            }

            await blobClient.UploadAsync(file.OpenReadStream(), true);
        }
        public async Task<List<string>> GetFiles()
        {
            await GetContainerClient();

            var blobNames = new List<string>();
            var blobs = _containerClient?.GetBlobsAsync();
            if (blobs is null)
            {
                return [];
            }

            await foreach (var blobItem in blobs)
            {
                blobNames.Add(blobItem.Name);
            }
            return blobNames;
        }
        public async Task<(MemoryStream, string)> DownloadFileAsync(string fileName)
        {
            await GetContainerClient();

            var blobClient = _containerClient?.GetBlobClient(fileName);
            if (blobClient is null)
            {
                throw new NullException("Blob client not found");
            }

            var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);

            memoryStream.Position = 0;

            var contentType = blobClient.GetProperties().Value.ContentType;
            return (memoryStream, contentType);
        }
        public async Task DeleteFileAsync(string fileName)
        {
            await GetContainerClient();

            var blobClient = _containerClient?.GetBlobClient(fileName);
            if (blobClient is null)
            {
                throw new NullException("Blob client not found");
            }

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
