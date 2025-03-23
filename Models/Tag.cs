namespace SlideCloud.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Tag
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    // رابطه چند به چند با Document از طریق جدول میانی TagDocument
    public ICollection<TagDocument> TagDocuments { get; set; }
}

