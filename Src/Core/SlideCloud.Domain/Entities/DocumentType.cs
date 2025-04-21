using System.ComponentModel.DataAnnotations.Schema;

namespace SlideCloud.Domain.Entities;

[Table("DocumentTypes")]
public class DocumentType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Document> Documents { get; set; }
}