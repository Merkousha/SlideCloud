using System.Collections.Generic;
using SlideCloud.Application.Services;
using SlideCloud.Application.DTOs.Category;

namespace SlideCloud.Application.DTOs
{
    public class ListSlideDTO
    {
        public PaginatedList<SlideDTO> Pagination { get; set; }
        public IEnumerable<CategoryDTO> DocumentCategories { get; set; }
    }
} 