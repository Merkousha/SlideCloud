using Microsoft.AspNetCore.Identity;

namespace SlideCloud.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User:IdentityUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string Email { get; set; }

    [Required]
    [MaxLength(50)]
    public string Password { get; set; }

    // شماره تلفن بهتر است string باشد تا بتواند فرمت‌های متفاوت را نگهداری کند
    [MaxLength(20)]
    public string PhoneNumber { get; set; }

    // کلید خارجی به Role
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}

