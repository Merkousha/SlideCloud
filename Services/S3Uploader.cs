using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using SlideCloud.Areas;

namespace SlideCloud.Services
{
    public class S3Uploader: IS3Uploader
    {
        private readonly IConfiguration _configuration;

        public S3Uploader(IConfiguration configuration)
        {
            _configuration = configuration;
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
    }
}
