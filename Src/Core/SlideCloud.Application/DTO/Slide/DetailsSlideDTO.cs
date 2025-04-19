using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.DTO.DocumentType;

namespace SlideCloud.Application.DTO.Slide
{
    public class DetailsSlideDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? File { get; set; }

        public string? Picture { get; set; }

        [Required(ErrorMessage = "Document Type is required")]
        public int DocumentTypeId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int DocumentCategoryId { get; set; }

        public IFormFile? NewFile { get; set; }

        public IFormFile? NewPicture { get; set; }

        public IEnumerable<CategoryDTO>? Categories { get; set; }
        public IEnumerable<DocumentTypeDTO>? Types { get; set; }

        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsShared { get; set; }

        public DocumentTypeDTO? DocumentType { get; set; }
        public CategoryDTO? DocumentCategory { get; set; }

        // This is a convenience property that provides the same data as the main DTO
        // for backward compatibility with views
        public DetailsSlideDTO DocumentDetail => this;
    }
}