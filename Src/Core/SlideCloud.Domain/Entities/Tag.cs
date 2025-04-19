using System.Collections.Generic;

namespace SlideCloud.Domain.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<TagDocument> TagDocuments { get; set; }
} 