using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.Pagination;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.DTO.DocumentType;

namespace SlideCloud.Application.Interfaces
{
    public interface ISlideService
    {
        Task<PaginationModel<Document>> GetSlidesAsync(int pageIndex, int? categoryId, int pageSize);
        Task<DetailsSlideDTO> GetSlideByIdAsync(int id);
        Task<DetailsSlideDTO> GetSlideDetailAsync(int id);
        Task IncrementViewCountAsync(int id);
        Task<PaginationModel<Document>> GetUserSlidesAsync(string userId, int pageIndex, int pageSize);
        Task<string> GetAuthorNameAsync(string userId);
        Task<IEnumerable<Document>> GetAuthorSlidesAsync(string userId);
        Task<byte[]> ConvertSlideToImageAsync(string filePath);
        Task CreateSlideAsync(SlideCreateDTO slide);
        Task UpdateSlideAsync(SlideUpdateDTO slide);
        Task DeleteSlideAsync(int id);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<DocumentTypeDTO>> GetAllDocumentTypesAsync();
    }
}