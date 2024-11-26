using ProjectBase.Insfracstructure.Services.Azure.Blob;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class AzureEndPoints
    {
        public static void MapAzurePoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/azure");

            group.MapPost("upload", UploadFile);

            group.MapGet("", GetFiles);

            group.MapGet("download", DownloadFiles);

            group.MapDelete("", DeleteFiles);
        }

        public static async Task<IResult> UploadFile(IFormFile file, IBlobService _blobService)
        {
            await _blobService.UploadFile(file);
            return Results.Ok("Upload file successfully");
        }

        public static async Task<IResult> GetFiles(IBlobService _blobService)
        {
            var res = await _blobService.GetFiles();
            return Results.Ok(res);
        }

        public static async Task<IResult> DownloadFiles(string fileName, IBlobService _blobService)
        {
            var (memoryStream, contentType) = await _blobService.DownloadFileAsync(fileName);
            return Results.File(memoryStream, contentType, fileName);
        }

        public static async Task<IResult> DeleteFiles(string fileName, IBlobService _blobService)
        {
            await _blobService.DeleteFileAsync(fileName);
            return Results.Ok();
        }
    }
}
