using System.ComponentModel.DataAnnotations.Schema;

namespace SlideCloud.Domain.Entities;

[Table("TagDocuments")]

public class TagDocument
{
    public int Id { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
    public int DocumentId { get; set; }
    public Document Document { get; set; }
}