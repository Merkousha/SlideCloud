using System.ComponentModel.DataAnnotations.Schema;

namespace SlideCloud.Domain.Entities;

[Table("Tags")]
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<TagDocument> TagDocuments { get; set; }
}