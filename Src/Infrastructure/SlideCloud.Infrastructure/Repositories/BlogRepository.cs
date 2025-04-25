using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Models.Blog;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Infrastructure.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly AppDbContext _context;

    public BlogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Blog> GetByIdAsync(long id)
    {
        return await _context.Blogs
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    // Implementation for IRepository<Blog>.GetByIdAsync(int id)
    async Task<Blog> IRepository<Blog>.GetByIdAsync(int id)
    {
        return await _context.Blogs
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public IQueryable<Blog> GetAll()
    {
        return _context.Blogs
            .Include(b => b.Author);
    }

    public IQueryable<Blog> Find(Expression<Func<Blog, bool>> expression)
    {
        return _context.Blogs
            .Include(b => b.Author)
            .Where(expression);
    }

    public async Task AddAsync(Blog entity)
    {
        await _context.Blogs.AddAsync(entity);
    }

    public void Update(Blog entity)
    {
        _context.Blogs.Update(entity);
    }

    public void Delete(Blog entity)
    {
        _context.Blogs.Remove(entity);
    }

    public async Task<Blog> GetBySlugAsync(string slug)
    {
        return await _context.Blogs
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Slug == slug);
    }

    public async Task<IEnumerable<Blog>> GetByAuthorIdAsync(long authorId)
    {
        return await _context.Blogs
            .Include(b => b.Author)
            .Where(b => b.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Blog>> GetPublishedBlogsAsync()
    {
        return await _context.Blogs
            .Include(b => b.Author)
            .Where(b => b.IsPublished)
            .OrderByDescending(b => b.PublishedAt)
            .ToListAsync();
    }
}
