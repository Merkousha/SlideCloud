using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Areas.Admin.Models.Category
{
    public class UpdateCategoryVM
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Slug { get; set; }
    }
}
