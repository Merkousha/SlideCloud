using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SlideCloud.Models;

namespace SlideCloud.Areas.User.Models.Slides
{
    public class UpdateSlides
    {
        public int Id { get; set; }
        [Display(Name = "توضیحات")]
        [MaxLength(5000, ErrorMessage = "متن زیادی بلد است")]
        public string Description { get; set; }

        [Required()]
        [MaxLength(255)]
        [Display(Name = "نام")]

        public string Title { get; set; }

        [MaxLength(255)]
        [Display(Name = "تصویر")]
        public string Picture { get; set; }

        [MaxLength(255)]
        [Display(Name = "فایل")]
        public string File { get; set; }

        [Display(Name = "نوع فایل")]
        // کلید خارجی به DocumentType
        public int DocumentTypeId { get; set; }

        [Display(Name = "دسته بندی")]
        // کلید خارجی به DocumentCategory
        public int DocumentCategoryId { get; set; }


        public int ViewCount { get; set; }

        //public ICollection<DocumentType> DocumentTypes { get; set; }
        //public ICollection<DocumentCategory> DocumentCategories { get; set; }



    }
}
