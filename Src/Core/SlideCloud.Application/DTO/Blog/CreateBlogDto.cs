using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Application.DTO.Blog;

public class CreateBlogDto
{
    [Required(ErrorMessage = "عنوان پست الزامی است")]
    [StringLength(200, ErrorMessage = "عنوان نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد")]
    public string Title { get; set; }

    [Required(ErrorMessage = "خلاصه پست الزامی است")]
    [StringLength(500, ErrorMessage = "خلاصه نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد")]
    public string Summary { get; set; }

    public string Content { get; set; }
    public string ImageUrl { get; set; }
} 