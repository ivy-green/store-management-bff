using Microsoft.AspNetCore.Http;

namespace ProjectBase.Insfracstructure.Services.Azure.Blob
{
    public interface IBlobService
    {
        Task UploadFile(IFormFile file);
        Task<List<string>> GetFiles();
        Task<(MemoryStream, string)> DownloadFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
    }
}