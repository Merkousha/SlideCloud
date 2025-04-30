namespace SlideCloud.Domain.Entities;

public class Blog
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Slug { get; set; }
    public string Summary { get; set; }
    public string FeaturedImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public long AuthorId { get; set; }
    public User Author { get; set; }
}