using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Application.DTO.DocumentType
{
    public class DocumentTypeDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }
    }
} 