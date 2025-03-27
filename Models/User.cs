namespace SlideCloud.Models;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User: IdentityUser<long>
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string Email { get; set; }


    // شماره تلفن بهتر است string باشد تا بتواند فرمت‌های متفاوت را نگهداری کند
    [MaxLength(20)]
    public string PhoneNumber { get; set; }

    
}

