using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.Pagination;
using SlideCloud.Application.DTO.Category;

namespace SlideCloud.Web.Models
{
    public class ListSlideVM
    {
        public PaginationModel<Document> Pagination { get; set; }
        public IEnumerable<CategoryDTO> DocumentCategories { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
} 