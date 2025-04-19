namespace SlideCloud.Domain.Entities;

public class Document
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public string Picture { get; set; }
    public string File { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsShared { get; set; }

    public int DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; }

    public int DocumentCategoryId { get; set; }
    public DocumentCategory DocumentCategory { get; set; }

    public long? UserId { get; set; }
    public User User { get; set; }

    public ICollection<TagDocument> TagDocuments { get; set; }

    public Document()
    {
        CreatedAt = DateTime.UtcNow;
        ViewCount = 0;
        TagDocuments = new List<TagDocument>();
    }
}