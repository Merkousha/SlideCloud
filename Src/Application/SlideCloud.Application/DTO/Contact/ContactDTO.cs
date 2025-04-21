using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Application.DTO.Contact
{
    public class ContactDTO
    {
        [Required(ErrorMessage = "لطفا نام و نام خانوادگی را وارد کنید")]
        [Display(Name = "نام و نام خانوادگی")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا شماره تماس را وارد کنید")]
        [Phone(ErrorMessage = "شماره تماس وارد شده معتبر نیست")]
        [Display(Name = "شماره تماس")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "لطفا پیام را وارد کنید")]
        [Display(Name = "پیام")]
        public string Message { get; set; }
    }
} 