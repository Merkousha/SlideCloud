using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.Services;

namespace SlideCloud.Application.DTO.Slide
{
    public class ListSlideDTO
    {
        public PaginatedList<SlideDTO> Pagination { get; set; }
        public IEnumerable<CategoryDTO> DocumentCategories { get; set; }
    }
} 