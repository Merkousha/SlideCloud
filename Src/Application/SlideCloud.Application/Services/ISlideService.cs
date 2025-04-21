using System.Collections.Generic;
using System.Threading.Tasks;
using SlideCloud.Application.DTOs;
using SlideCloud.Application.Services;

namespace SlideCloud.Application.Services
{
    public interface ISlideService
    {
        Task<PaginatedList<SlideDTO>> GetAllSlidesAsync(int pageIndex = 1, int? categoryId = null, string searchTerm = null, int pageSize = 12);
    }
} 