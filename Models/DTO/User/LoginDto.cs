using System.ComponentModel.DataAnnotations;
using SlideCloud.Controller;

namespace SlideCloud.Models.DTO.User
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class RegisterModel
    {
        [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "تکرار رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن باید یکسان باشند.")]
        public string ConfirmPassword { get; set; }
    }
    public class RegisterDto
    {
     
        [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "نام باید فقط شامل حروف فارسی یا انگلیسی باشد.")]

        [Required(ErrorMessage ="نام را واردکنید")]
        public string FullName { get; set; }


        [Required(ErrorMessage = "ایمیل خود را وارد کنید")]
        [EmailAddress(ErrorMessage ="فرمت ایمیل درست نیست")]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage ="رمز خود را وارد کن")]
        [MinLength(4,ErrorMessage ="حداقل باید 4 کلمه باشد")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز خود را وارد کن")]
        [MinLength(4, ErrorMessage = "حداقل باید 4 کلمه باشد")]
        [Compare(nameof(Password), ErrorMessage = "رمز عبور و تکرار آن باید یکسان باشند.")]
        public string RePassword { get; set; }

        [Required(ErrorMessage = "شماره خود را وارد کن")]

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
