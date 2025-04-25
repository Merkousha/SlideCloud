using SlideCloud.Domain.Models.Blog;

namespace SlideCloud.Domain.Interfaces;

public interface IBlogRepository : IRepository<Blog>
{
    Task<Blog> GetBySlugAsync(string slug);
    Task<IEnumerable<Blog>> GetByAuthorIdAsync(long authorId);
    Task<IEnumerable<Blog>> GetPublishedBlogsAsync();
    new Task<Blog> GetByIdAsync(long id);
}