using SlideCloud.Domain.Entities;

namespace SlideCloud.Domain.Models.Blog;

public class Blog
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Slug { get; set; }
    public string Summary { get; set; }
    public string FeaturedImage { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    // Foreign key for the author
    public long? AuthorId { get; set; }

    // Navigation property for the author
    public User? Author { get; set; }
}