using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Application.Services;

public class BlogService : IBlogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlogRepository _blogRepository;
    private readonly IConfiguration _configuration;
    private readonly Kernel _kernel;

    public BlogService(IBlogRepository blogRepository, IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _blogRepository = blogRepository;
        _configuration = configuration;
        _kernel = CreateKernel();
        _unitOfWork = unitOfWork;
    }

    private Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(
            modelId: "gpt-4o-2024-11-20",
            apiKey: _configuration["OpenAI:ApiKey"]!);
        return builder.Build();
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
        return _unitOfWork.Blogs.GetAll().ToList();
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
        var blog = await _blogRepository.GetByIdAsync(id);
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

    public async Task<(string Content, string ImageUrl)> GenerateAIContentAsync(string title, string summary)
    {
        var prompt = $@"Generate a blog post with the following title and summary. The response should be in JSON format with the following structure:
        {{
            ""content"": ""The main content of the blog post"",
            ""imagePrompt"": ""A detailed prompt for generating an image that represents the blog post""
        }}

        Title: {title}
        Summary: {summary}";

        var result = await _kernel.InvokePromptAsync(prompt);
        var response = JsonSerializer.Deserialize<BlogGenerationResponse>(result.ToString());

        if (response == null)
        {
            throw new Exception("Failed to generate blog content");
        }

        // For now, we'll return a placeholder image URL since image generation is not yet implemented
        var imageUrl = "https://placeholder.com/800x600";

        return (response.Content, imageUrl);
    }
}

public class BlogGenerationResponse
{
    public string Content { get; set; } = string.Empty;
    public string ImagePrompt { get; set; } = string.Empty;
}