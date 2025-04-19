using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Application.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadPath;

        public FileService(IConfiguration configuration)
        {
            _uploadPath = configuration["FileStorage:UploadPath"] ?? "wwwroot/uploads";
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null", nameof(file));

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_uploadPath, filePath);
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_uploadPath, filePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"File not found: {filePath}");

            return await File.ReadAllBytesAsync(fullPath);
        }

        public bool IsValidFileType(IFormFile file, string[] allowedExtensions)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
    }
}