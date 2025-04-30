using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Application.Services
{
    public class S3Uploader : IS3Uploader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Uploader()
        {
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var bucketName = Environment.GetEnvironmentVariable("S3-BucketName");
            var accessKey = Environment.GetEnvironmentVariable("S3-AccessKey");
            var secretKey = Environment.GetEnvironmentVariable("S3-SecretKey");
            var serviceUrl = Environment.GetEnvironmentVariable("S3-ServiceURL");

            var config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
                UseHttp = true
            };

            using (var client = new AmazonS3Client(accessKey, secretKey, config))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(newMemoryStream);
                    newMemoryStream.Position = 0;

                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = fileName,
                        BucketName = bucketName,
                        ContentType = file.ContentType,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return $"{serviceUrl}/{bucketName}/{fileName}".Replace("//", "/").Replace(":/", "://");
                }
            }
        }

        public async Task<string> UploadFileAsync(MemoryStream fileMemoryStram)
        {
            var bucketName = Environment.GetEnvironmentVariable("S3-BucketName");
            var accessKey = Environment.GetEnvironmentVariable("S3-AccessKey");
            var secretKey = Environment.GetEnvironmentVariable("S3-SecretKey");
            var serviceUrl = Environment.GetEnvironmentVariable("S3-ServiceURL");

            var config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
                UseHttp = true
            };

            using (var client = new AmazonS3Client(accessKey, secretKey, config))
            {
                using (fileMemoryStram)
                {
                    fileMemoryStram.Position = 0;

                    var fileName = $"{Guid.NewGuid()}_blog.png";

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = fileMemoryStram,
                        Key = fileName,
                        BucketName = bucketName,
                        ContentType = MediaTypeNames.Image.Png,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return $"{serviceUrl}/{bucketName}/{fileName}".Replace("//", "/").Replace(":/", "://");
                }
            }
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = filePath
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = filePath
            };

            using var response = await _s3Client.GetObjectAsync(getRequest);
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
