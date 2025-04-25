using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Models.Blog;

namespace SlideCloud.Application.Services;

public class BlogService : IBlogService
{
    private readonly IUnitOfWork _unitOfWork;

    public BlogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Blog> GetBlogByIdAsync(long id)
    {
        return await _unitOfWork.Blogs.GetByIdAsync(id);
    }

    public async Task<Blog> GetBlogBySlugAsync(string slug)
    {
        return await _unitOfWork.Blogs.GetBySlugAsync(slug);
    }

    public async Task<IEnumerable<Blog>> GetAllBlogsAsync()
    {
        return await _unitOfWork.Blogs.GetAll().ToListAsync();
    }

    public async Task<IEnumerable<Blog>> GetUserBlogsAsync(long userId)
    {
        return await _unitOfWork.Blogs.GetByAuthorIdAsync(userId);
    }

    public async Task<IEnumerable<Blog>> GetPublishedBlogsAsync()
    {
        return await _unitOfWork.Blogs.GetPublishedBlogsAsync();
    }

    public async Task<Blog> CreateBlogAsync(Blog blog, long authorId)
    {
        blog.AuthorId = authorId;
        blog.CreatedAt = DateTime.UtcNow;
        blog.IsPublished = false;

        await _unitOfWork.Blogs.AddAsync(blog);
        await _unitOfWork.SaveChangesAsync();

        return blog;
    }

    public async Task<Blog> UpdateBlogAsync(long id, Blog blog, long userId)
    {
        var existingBlog = await _unitOfWork.Blogs.GetByIdAsync(id);
        if (existingBlog == null)
        {
            throw new KeyNotFoundException($"Blog with ID {id} not found.");
        }

        if (existingBlog.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this blog.");
        }

        existingBlog.Title = blog.Title;
        existingBlog.Content = blog.Content;
        existingBlog.Slug = blog.Slug;
        existingBlog.Summary = blog.Summary;
        existingBlog.FeaturedImage = blog.FeaturedImage;
        existingBlog.IsPublished = blog.IsPublished;
        existingBlog.UpdatedAt = DateTime.UtcNow;

        if (blog.IsPublished && !existingBlog.PublishedAt.HasValue)
        {
            existingBlog.PublishedAt = DateTime.UtcNow;
        }

        _unitOfWork.Blogs.Update(existingBlog);
        await _unitOfWork.SaveChangesAsync();

        return existingBlog;
    }

    public async Task DeleteBlogAsync(long id, long userId)
    {
        var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
        if (blog == null)
        {
            throw new KeyNotFoundException($"Blog with ID {id} not found.");
        }

        if (blog.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this blog.");
        }

        _unitOfWork.Blogs.Delete(blog);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsBlogAuthorAsync(long blogId, long userId)
    {
        var blog = await _unitOfWork.Blogs.GetByIdAsync(blogId);
        return blog?.AuthorId == userId;
    }
} 