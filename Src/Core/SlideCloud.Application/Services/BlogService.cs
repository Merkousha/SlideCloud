using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using OpenAI;
using SlideCloud.Application.DTO.Presentation;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Models.Blog;

namespace SlideCloud.Application.Services;

public class BlogService : IBlogService
{
    private readonly IUnitOfWork _unitOfWork;
    string modelId = "gpt-4o-2024-11-20";
    string endpoint = "https://api.avalai.ir/v1";
    string apiKey = "aa-VckNj9CcU1uLMczXGigPhavLokYY4V2meOFzglIDDq3j6kIJ";
    // Create a history store the conversation
    ChatHistory history = new ChatHistory();
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
        if (blog.Summary == "*****")
        { 
            blog.Content = await GenerateBlogPostAsync(blog.Title);
        }
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

    private async Task<string> GenerateBlogPostAsync(string mainTitle)
    {
        // Simple slug generation logic
        // Create a kernel with Azure OpenAI chat completion
        var openAiClientOption = new OpenAIClientOptions
        {
            Endpoint = new Uri(endpoint),
        };
        var apiCredential = new System.ClientModel.ApiKeyCredential(apiKey);
        var openAiClient = new OpenAIClient(apiCredential, openAiClientOption);
        var builder = Kernel.CreateBuilder();
        builder.Services.AddOpenAIChatCompletion(modelId, openAiClient);

        // Build the kernel
        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Enable planning
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        // مرحله دوم: تولید پست HTML با Bootstrap 5
        var htmlPrompt = @$"
    [system]
    شما یک نویسنده حرفه‌ای وبلاگ هستید. بر اساس موضوعی که توسط کاربر ارائه می‌شود، یک پست وبلاگ کامل و جذاب بنویس که برای خواننده قابل فهم و از نظر سئو بهینه باشد.

    خروجی شما باید فقط و فقط شامل محتوای HTML پست باشد.  
    از ساختار HTML و کلاس‌های Bootstrap 5 استفاده کن تا نتیجه ظاهری زیبا و ریسپانسیو داشته باشد.

    راهنمایی‌های مهم:
    - خروجی را فقط در قالب تگ‌های HTML ارائه بده.
    - از کلاس‌هایی مانند `container`, `row`, `col`, `card`, `lead` و دیگر کلاس‌های Bootstrap 5 به‌صورت مناسب استفاده کن.
    - عنوان اصلی پست را در یک تگ `<h1 class=""mb-4"">` قرار بده.
    - از پاراگراف‌ها و هدینگ‌های فرعی مانند `<h2>` و `<h3>` برای دسته‌بندی مطالب استفاده کن.
    - هیچ متن یا توصیفی خارج از ساختار HTML و پست تولید نکن. هیچ تفسیر یا توضیحی نده.

    [user]
    موضوع پست: {mainTitle}
    ";

        // ارسال درخواست دوم
        ChatHistory htmlHistory = new();
        htmlHistory.AddSystemMessage(htmlPrompt);
        htmlHistory.AddUserMessage($"موضوع پست: {mainTitle}");

        var htmlResult = await chatCompletionService.GetChatMessageContentAsync(
            htmlHistory,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);

        // خروجی HTML نهایی
        string blogHtml = htmlResult.Content ?? throw new InvalidOperationException("No blog HTML generated");

        // در اینجا می‌تونی HTML رو در یک فایل ذخیره کنی یا برگردونی
        return blogHtml;
    }
} 