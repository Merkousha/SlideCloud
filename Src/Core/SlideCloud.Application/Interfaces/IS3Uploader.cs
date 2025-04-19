using Microsoft.AspNetCore.Http;

namespace SlideCloud.Application.Interfaces
{
    public interface IS3Uploader
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string filePath);
        Task<byte[]> GetFileAsync(string filePath);
    }
} 