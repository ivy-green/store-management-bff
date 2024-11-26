using Microsoft.AspNetCore.Http;

namespace ProjectBase.Insfracstructure.Services.FileService
{
    public interface IFileService
    {
        Task CreateBucket(string bucket);
        Task UploadFileS3(string bucket, IFormFile file);
        Task<List<string>> GetAllBucket();
        Task DeleteBucket(string bucketName);
        Task<(MemoryStream, string)> DownloadFileFromBucket(string bucketName, string fileName);
        Task<string> GetFileUrlS3(string bucket);
    }
}