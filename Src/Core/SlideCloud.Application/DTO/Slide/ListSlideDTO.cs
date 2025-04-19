using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.Pagination;

namespace SlideCloud.Application.DTO.Slide;

public class ListSlideDTO
{
    public required PaginationModel<Document> Pagination { get; set; }
    public required IEnumerable<DocumentCategory> DocumentCategories { get; set; }
} 