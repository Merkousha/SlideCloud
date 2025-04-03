using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Areas.Admin.Models.Category
{
    public class UpdateCategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="{0} را وارد کنید")]
        [MaxLength(50)]
        [Display(Name ="نام")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(255)]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(50)]
        [Display(Name = "عنوان در مرورگر")]
        public string Slug { get; set; }
    }
}
