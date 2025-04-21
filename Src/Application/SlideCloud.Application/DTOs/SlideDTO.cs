using System;

namespace SlideCloud.Application.DTOs
{
    public class SlideDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string ThumbnailPath { get; set; }
        public int DocumentCategoryId { get; set; }
        public string DocumentCategoryName { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 