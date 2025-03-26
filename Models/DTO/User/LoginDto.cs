namespace SlideCloud.Models.DTO.User
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
