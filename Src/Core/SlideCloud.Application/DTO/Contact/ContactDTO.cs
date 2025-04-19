using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Application.DTO.Contact
{
    public class ContactDTO
    {
        [Required(ErrorMessage = "لطفا نام و نام خانوادگی خود را وارد کنید")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "لطفا ایمیل خود را وارد کنید")]
        [EmailAddress(ErrorMessage = "لطفا یک ایمیل معتبر وارد کنید")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا شماره تماس خود را وارد کنید")]
        [Phone(ErrorMessage = "لطفا یک شماره تماس معتبر وارد کنید")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "لطفا پیام خود را وارد کنید")]
        public string Message { get; set; }
    }
} 