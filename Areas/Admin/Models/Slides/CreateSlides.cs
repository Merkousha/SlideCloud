using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SlideCloud.Models;

namespace SlideCloud.Areas.Admin.Models.Slides
{
    public class CreateSlides
    {

        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Picture { get; set; }

        [MaxLength(255)]
        public string File { get; set; }

        // کلید خارجی به DocumentType
        public int DocumentTypeId { get; set; }


        // کلید خارجی به DocumentCategory
        public int DocumentCategoryId { get; set; }

        public ICollection<DocumentType> DocumentTypes { get; set; }
        public ICollection<DocumentCategory> DocumentCategories { get; set; }

    }
}
