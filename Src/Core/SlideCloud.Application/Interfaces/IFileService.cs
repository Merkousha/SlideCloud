using Microsoft.AspNetCore.Http;

namespace SlideCloud.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string filePath);
        Task<byte[]> GetFileAsync(string filePath);
        bool IsValidFileType(IFormFile file, string[] allowedExtensions);
    }
} 