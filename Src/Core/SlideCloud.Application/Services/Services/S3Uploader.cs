using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Application.Services.Services
{
    public class S3Uploader : IS3Uploader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Uploader(IAmazonS3 s3Client, string bucketName)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = memoryStream,
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);
            return fileName;
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
