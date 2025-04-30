using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenAI;
using OpenAI.Images;
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
#pragma warning disable SKEXP0010
        builder.AddOpenAIChatCompletion(
            modelId: "gpt-4o-2024-11-20",
            apiKey: "aa-VckNj9CcU1uLMczXGigPhavLokYY4V2meOFzglIDDq3j6kIJ",
            endpoint: new Uri("https://api.avalai.ir/v1") // Replace with your actual endpoint
            );

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
        blog.IsPublished = true;
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
        var prompt = $@"Generate a blog post (in persian) with the following title and summary. The response should be in JSON format with the following structure:
        {{
            ""Content"": ""The main HTML content of the post "",
            ""ImagePrompt"": ""A detailed prompt for generating an image that represents the blog post""
        }}

        the Content field should only contain the HTML content of the post.
Use HTML structure and Bootstrap 5 classes to create a beautiful and responsive result.
Important tips:
- Provide output only in HTML tags.
- Use classes like `container`, `row`, `col`, `card`, `lead` and other Bootstrap 5 classes appropriately.
- Put the main title of the post in a `<h1 class=""mb-4"">` tag.
- Use paragraphs and subheadings like `<h2>` and `<h3>` to categorize content.
- Do not generate any text or description outside the HTML structure and post. Do not provide any commentary or explanation.
        Title: {title}
        Summary: {summary}";

        var result = await _kernel.InvokePromptAsync(prompt);
        var cleanedResult = CleanChatMessageContent(result.ToString());
        var response = JsonSerializer.Deserialize<BlogGenerationResponse>(cleanedResult);

        if (response == null)
        {
            throw new Exception("Failed to generate blog content");
        }

        // For now, we'll return a placeholder image URL since image generation is not yet implemented
        var imageUrl = await GenerateImageAsync(response.ImagePrompt);

        return (response.Content, imageUrl);
    }

    private async Task<string> GenerateImageAsync(string prompt)
    {
        ApiKeyCredential key = new ApiKeyCredential("aa-VckNj9CcU1uLMczXGigPhavLokYY4V2meOFzglIDDq3j6kIJ");
        OpenAIClientOptions options = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.avalai.ir/v1")
        };
        ImageClient client = new("dall-e-3", key, options);

        // Generate the image
        GeneratedImage generatedImage = await client.GenerateImageAsync(prompt,
      new ImageGenerationOptions
      {
          Size = GeneratedImageSize.W1024xH1024
      });

        // Save the image to a file or return the URL
        // Convert the stream to a byte array


        return generatedImage.ImageUri.AbsoluteUri;
    }


    private static string CleanChatMessageContent(string result)
    {
        return result
            .Replace("```", "")
            .Replace("json", "");
    }

    public class BlogGenerationResponse
    {
        public string Content { get; set; } = string.Empty;
        public string ImagePrompt { get; set; } = string.Empty;
    }

}
