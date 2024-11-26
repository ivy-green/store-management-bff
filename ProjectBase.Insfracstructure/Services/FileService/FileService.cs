using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace ProjectBase.Insfracstructure.Services.FileService
{
    [ExcludeFromCodeCoverage]
    public class FileService : IFileService
    {
        AppSettingConfiguration _setting;
        private readonly IAmazonS3 _s3Client;
        public FileService(AppSettingConfiguration setting, IAmazonS3 s3Client)
        {
            _setting = setting;
            _s3Client = s3Client;
        }
        public async Task UploadFileS3(string bucket, IFormFile file)
        {
            /*var config = new AmazonS3Config() 
            { 
                ServiceURL = _setting.AWSSection.S3Url ,
                RegionEndpoint = RegionEndpoint.USEast1
            };
            var client = new AmazonS3Client(_setting.AWSSection.AccessKey, _setting.AWSSection.Secret, config);*/
            await CreateBucket(bucket);

            var objectRequest = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = file.FileName,
                InputStream = file.OpenReadStream(),
            };
            await _s3Client.PutObjectAsync(objectRequest);
        }

        public async Task<string> GetFileUrlS3(string bucket)
        {
            if (!(await IsBucketExists(bucket)))
            {
                throw new BucketNotFoundException();
            }

            var objectRequest = new GetPreSignedUrlRequest()
            {
                BucketName = bucket,
                Expires = DateTime.UtcNow.AddHours(24) // fix
            };
            var res = await _s3Client.GetPreSignedURLAsync(objectRequest);

            return res;
        }

        public async Task CreateBucket(string bucket)
        {
            var buckExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucket);
            if (!buckExists)
            {
                var bucketRequest = new PutBucketRequest()
                {
                    BucketName = bucket,
                    UseClientRegion = true,
                };
                var response = await _s3Client.PutBucketAsync(bucketRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Create bucket failed!");
                }
            }
        }

        private async Task<bool> IsBucketExists(string bucketName)
        {
            var buckExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            return buckExists ? true : false;
        }

        public async Task<List<string>> GetAllBucket()
        {
            var buckets = await _s3Client.ListBucketsAsync();
            var res = buckets.Buckets.Select(x => x.BucketName).ToList();
            return res;
        }

        public async Task DeleteBucket(string bucketName)
        {
            await _s3Client.DeleteBucketAsync(bucketName);
        }

        public async Task<(MemoryStream, string)> DownloadFileFromBucket(string bucketName, string fileName)
        {
            GetObjectRequest objectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
            };

            MemoryStream? memoryStream = null;
            var mimeType = "";
            using (var res = await _s3Client.GetObjectAsync(objectRequest))
            {
                if (res.HttpStatusCode == HttpStatusCode.OK)
                {
                    using (memoryStream = new MemoryStream())
                    {
                        await res.ResponseStream.CopyToAsync(memoryStream);
                        mimeType = res.Headers.ContentType;
                    }
                }
            }

            if (memoryStream is null || memoryStream.ToArray().Length < 1)
            {
                throw new Domain.Exceptions.FileNotFoundException();
            }

            return (memoryStream, mimeType);
        }
    }
}
