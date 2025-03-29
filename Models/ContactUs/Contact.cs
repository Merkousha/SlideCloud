using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Models.ContactUs
{
    public class Contact
    {
        public int Id { get; set; }
        [DisplayName("نام و نام خانوادگی")]
        [Required(ErrorMessage = "این فیلد اجباریست")]
        [MinLength(3, ErrorMessage = "حداقل طول نام 3 کاراکتر است.")]
        [MaxLength(50, ErrorMessage = "حداکثر طول نام 50 کاراکتر است")]
        public string FullName { get; set; }

        [DisplayName("ایمیل")]
        [Required(ErrorMessage = "این فیلد اجباریست")]
        [EmailAddress(ErrorMessage = "آدرس ایمیل وارد شده صحیح نمی باشد.")]
        public string Email { get; set; }

        [DisplayName("شماره تماس")]
        [Phone(ErrorMessage = "شماره تماس وارد شده صحیح نمی باشد.")]
        public string PhoneNumber { get; set; }

        [DisplayName("متن پبام")]
        [Required(ErrorMessage = ".متن پیام نمیتواند خالی باشد")]
        public string Message { get; set; }

        public Contact() { }

        public Contact(string fullName, string email, string phoneNumber, string message)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            Message = message;
        }
    }


}
