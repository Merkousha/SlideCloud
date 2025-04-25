using SlideCloud.Domain.Models.Blog;

namespace SlideCloud.Application.Interfaces;

public interface IBlogService
{
    Task<Blog> GetBlogByIdAsync(long id);
    Task<Blog> GetBlogBySlugAsync(string slug);
    Task<IEnumerable<Blog>> GetAllBlogsAsync();
    Task<IEnumerable<Blog>> GetUserBlogsAsync(long userId);
    Task<IEnumerable<Blog>> GetPublishedBlogsAsync();
    Task<Blog> CreateBlogAsync(Blog blog, long authorId);
    Task<Blog> UpdateBlogAsync(long id, Blog blog, long userId);
    Task DeleteBlogAsync(long id, long userId);
    Task<bool> IsBlogAuthorAsync(long blogId, long userId);
} 