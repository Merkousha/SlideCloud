using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Application.DTO.Blog;

public class UpdateBlogDto
{
    [Required(ErrorMessage = "عنوان پست الزامی است")]
    [StringLength(200, ErrorMessage = "عنوان نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد")]
    public string Title { get; set; }

    [Required(ErrorMessage = "خلاصه پست الزامی است")]
    [StringLength(500, ErrorMessage = "خلاصه نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد")]
    public string Summary { get; set; }

    [Required(ErrorMessage = "محتوای پست الزامی است")]
    public string Content { get; set; }

    [StringLength(500, ErrorMessage = "آدرس تصویر نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد")]
    public string FeaturedImage { get; set; }

    public bool IsPublished { get; set; }
} 