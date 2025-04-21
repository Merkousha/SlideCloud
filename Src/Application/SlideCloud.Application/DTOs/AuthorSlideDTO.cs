using System.Collections.Generic;

namespace SlideCloud.Application.DTOs
{
    public class AuthorSlideDTO
    {
        public string AuthorName { get; set; }
        public IEnumerable<SlideDTO> Slides { get; set; }
    }
} 