using System.ComponentModel.DataAnnotations;

namespace SlideCloud.Models.DTO.User
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(4)]
        public string Password { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class ForgetPasswordDto
    {
        [Required]
        public string Email { get; set; }
 
    }
}
