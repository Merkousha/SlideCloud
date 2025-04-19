using SlideCloud.Application.DTO.Category;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.Pagination;

namespace SlideCloud.Web.Areas.Admin.Models.Slides
{
    public class ListSlideVM
    {
        public PaginationModel<Document> Documents { get; set; }
        public IEnumerable<CategoryDTO> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}