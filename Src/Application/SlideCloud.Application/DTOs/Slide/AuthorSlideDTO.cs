using System.Collections.Generic;
using SlideCloud.Application.DTOs.Slide;

namespace SlideCloud.Application.DTOs.Slide
{
    public class AuthorSlideDTO
    {
        public string AuthorName { get; set; }
        public IEnumerable<SlideDTO> Slides { get; set; }
    }
} 