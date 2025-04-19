using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Application.DTO.Slide
{
    public class SlideUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string File { get; set; }

        public string Picture { get; set; }

        [Required(ErrorMessage = "Document Type is required")]
        public int DocumentTypeId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int DocumentCategoryId { get; set; }
    }
} 