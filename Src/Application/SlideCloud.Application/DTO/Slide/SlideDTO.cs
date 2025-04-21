using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.DTO.DocumentType;

namespace SlideCloud.Application.DTO.Slide
{
    public class SlideDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public CategoryDTO Category { get; set; }
        public DocumentTypeDTO DocumentType { get; set; }
    }
} 