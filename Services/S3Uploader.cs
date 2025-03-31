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
            var bucketName = _configuration["AWS:BucketName"];
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var region = Amazon.RegionEndpoint.GetBySystemName(_configuration["AWS:Region"]);

            using (var client = new AmazonS3Client(accessKey, secretKey, region))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(newMemoryStream);

                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = fileName,
                        BucketName = bucketName,
                        ContentType = file.ContentType,
                        CannedACL = S3CannedACL.PublicRead // فایل برای همه قابل دسترسی باشه
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);

                    // آدرس کامل فایل آپلود شده
                    return $"https://{bucketName}.s3.amazonaws.com/{fileName}";
                }
            }
        }
    }


}
