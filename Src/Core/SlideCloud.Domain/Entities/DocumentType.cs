using System.Collections.Generic;

namespace SlideCloud.Domain.Entities;

public class DocumentType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Document> Documents { get; set; }
} 