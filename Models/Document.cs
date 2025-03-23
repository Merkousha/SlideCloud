namespace SlideCloud.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Document
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Description { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [MaxLength(255)]
    public string Picture { get; set; }

    [MaxLength(255)]
    public string File { get; set; }

    // کلید خارجی به DocumentType
    public int DocumentTypeId { get; set; }
    [ForeignKey("DocumentTypeId")]
    public DocumentType DocumentType { get; set; }

    // کلید خارجی به DocumentCategory
    public int DocumentCategoryId { get; set; }
    [ForeignKey("DocumentCategoryId")]
    public DocumentCategory DocumentCategory { get; set; }

    public int ViewCount { get; set; }

    // رابطه چند به چند با Tag از طریق جدول میانی TagDocument
    public ICollection<TagDocument> TagDocuments { get; set; }
}

