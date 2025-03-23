namespace SlideCloud.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    // Navigation Property
    public ICollection<User> Users { get; set; }
}

