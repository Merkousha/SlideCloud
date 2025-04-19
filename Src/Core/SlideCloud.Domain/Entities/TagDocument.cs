namespace SlideCloud.Domain.Entities;

public class TagDocument
{
    public int Id { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
    public int DocumentId { get; set; }
    public Document Document { get; set; }
} 