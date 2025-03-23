namespace SlideCloud.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class DocumentCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Description { get; set; }

    [MaxLength(50)]
    public string Slug { get; set; }

    // Navigation Property
    [BindNever]
    public ICollection<Document> Documents { get; set; }
}

