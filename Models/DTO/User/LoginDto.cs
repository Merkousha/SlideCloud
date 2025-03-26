using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Models.DTO.User
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="نام را واردکنید")]
        public string FullName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage ="فرمت ایمیل درست نیست")]
        public string Email { get; set; }
        [Required(ErrorMessage ="رمز خود را وارد کن")]
        [MinLength(4,ErrorMessage ="حداقل باید 4 کلمه باشد")]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "لطفاً یک شماره تلفن همراه معتبر ایرانی وارد کنید.")]
        public string PhoneNumber { get; set; }
    }

    public class LoginDto
    {
        [EmailAddress(ErrorMessage = "فرمت ایمیل درست نیست")]
        [Required(ErrorMessage ="ایمیل خود را وارد کنید")]
        public string Email { get; set; }
        [Required(ErrorMessage = "رمز خود را وارد کن")]
        public string Password { get; set; }
    }
    public class ForgetPasswordDto
    {
        [EmailAddress(ErrorMessage = "فرمت ایمیل درست نیست")]
        [Required(ErrorMessage = "ایمیل خود را وارد کنید")]
        public string Email { get; set; }
 
    }
}
