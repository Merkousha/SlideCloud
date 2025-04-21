using System.Collections.Generic;
using System.Threading.Tasks;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.Services;

namespace SlideCloud.Application.Interfaces
{
    public interface ISlideService
    {
        Task<PaginatedList<SlideDTO>> GetAllSlidesAsync(int pageIndex = 1, int? categoryId = null, string searchTerm = null, int pageSize = 12);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<SlideDTO> GetSlideDetailAsync(int id);
        Task IncrementViewCountAsync(int id);
        Task<string> GetAuthorNameAsync(string userId);
        Task<IEnumerable<SlideDTO>> GetAuthorSlidesAsync(string userId);
        Task<PaginatedList<SlideDTO>> GetUserSlidesAsync(string userId, int pageIndex = 1, int pageSize = 12);
        Task<Stream> ConvertSlideToImageAsync(string filePath);
    }
} 