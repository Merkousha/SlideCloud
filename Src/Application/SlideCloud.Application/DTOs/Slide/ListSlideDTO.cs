using System.Collections.Generic;
using SlideCloud.Application.DTOs.Category;
using SlideCloud.Application.Services;

namespace SlideCloud.Application.DTOs.Slide
{
    public class ListSlideDTO
    {
        public PaginatedList<SlideDTO> Pagination { get; set; }
        public IEnumerable<CategoryDTO> DocumentCategories { get; set; }
    }
} 