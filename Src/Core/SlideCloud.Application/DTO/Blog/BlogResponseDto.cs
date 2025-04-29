namespace SlideCloud.Application.DTO.Blog;

public class BlogResponseDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Slug { get; set; }
    public string Summary { get; set; }
    public string FeaturedImage { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public long AuthorId { get; set; }
    public string AuthorName { get; set; }

    public static BlogResponseDto FromBlog(Domain.Entities.Blog blog)
    {
        return new BlogResponseDto
        {
            Id = blog.Id,
            Title = blog.Title,
            Content = blog.Content,
            Slug = blog.Slug,
            Summary = blog.Summary,
            FeaturedImage = blog.FeaturedImage,
            IsPublished = blog.IsPublished,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt,
            PublishedAt = blog.PublishedAt,
            AuthorId = blog.AuthorId,
            AuthorName = blog.Author?.Name ?? "Unknown"
        };
    }
}