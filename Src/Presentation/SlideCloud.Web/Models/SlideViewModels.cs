using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.Pagination;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.DTO.Slide;

namespace SlideCloud.Web.Models
{
    public class SlideIndexViewModel
    {
        public PaginationModel<Document>? Pagination { get; set; }
        public IEnumerable<CategoryDTO>? Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
    }

    public class AuthorViewModel
    {
        public string? AuthorName { get; set; }
        public IEnumerable<Document>? Slides { get; set; }
    }
} 