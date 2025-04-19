using SlideCloud.Domain.Entities;

namespace SlideCloud.Application.DTO.Slide
{
    public class AuthorSlideDTO
    {
        public List<Document> Slides { get; set; }
        public string AuthorName { get; set; }
    }
} 