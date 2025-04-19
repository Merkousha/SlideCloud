using Microsoft.AspNetCore.Http;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Application.Interfaces
{
    public interface IAdminSlideService
    {
        Task<IEnumerable<Document>> GetAllSlidesAsync();
        Task<Document> GetSlideByIdAsync(int id);
        Task<int> CreateSlideAsync(Document document);
        Task UpdateSlideAsync(Document document);
        Task DeleteSlideAsync(int id);
        Task<bool> SlideExistsAsync(int id);
        Task<IEnumerable<DocumentType>> GetAllDocumentTypesAsync();
        Task<IEnumerable<DocumentCategory>> GetAllCategoriesAsync();
        Task<string> UploadFileAsync(IFormFile file, string type);
    }
}