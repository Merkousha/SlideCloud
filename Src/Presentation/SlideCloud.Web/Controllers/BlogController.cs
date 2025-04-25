using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Models.Blog;

namespace SlideCloud.Web.Controllers;

public class BlogController : Controller
{
    private readonly IBlogService _blogService;
    private readonly IUserService _userService;

    public BlogController(IBlogService blogService, IUserService userService)
    {
        _blogService = blogService;
        _userService = userService;
    }

    // GET: Blog
    public async Task<IActionResult> Index()
    {
        var blogs = await _blogService.GetPublishedBlogsAsync();
        return View(blogs);
    }

    // GET: Blog/MyBlogs
    [Authorize]
    public async Task<IActionResult> MyBlogs()
    {
        var currentUser = await _userService.GetCurrentUser();
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var blogs = await _blogService.GetUserBlogsAsync(currentUser.Id);
        return View(blogs);
    }

    // GET: Blog/Details/5
    public async Task<IActionResult> Details(long id)
    {
        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null)
        {
            return NotFound();
        }

        return View(blog);
    }

    // GET: Blog/Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Blog/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(Blog blog)
    {
        blog.Slug = GenerateSlug(blog.Title);
        if (ModelState.IsValid)
        {
            var currentUser = await _userService.GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            blog.IsPublished = true;
            await _blogService.CreateBlogAsync(blog, currentUser.Id);
            return RedirectToAction(nameof(MyBlogs));
        }

        return View(blog);
    }

    // GET: Blog/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(long id)
    {
        var currentUser = await _userService.GetCurrentUser();
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null)
        {
            return NotFound();
        }

        if (blog.AuthorId != currentUser.Id)
        {
            return Unauthorized();
        }

        return View(blog);
    }

    // POST: Blog/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(long id, Blog blog)
    {
        if (id != blog.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var currentUser = await _userService.GetCurrentUser();
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                blog.Slug = GenerateSlug(blog.Title);
                await _blogService.UpdateBlogAsync(id, blog, currentUser.Id);
                return RedirectToAction(nameof(MyBlogs));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        return View(blog);
    }

    // GET: Blog/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        var currentUser = await _userService.GetCurrentUser();
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null)
        {
            return NotFound();
        }

        if (blog.AuthorId != currentUser.Id)
        {
            return Unauthorized();
        }

        return View(blog);
    }

    // POST: Blog/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var currentUser = await _userService.GetCurrentUser();
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            await _blogService.DeleteBlogAsync(id, currentUser.Id);
            return RedirectToAction(nameof(MyBlogs));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private string GenerateSlug(string title)
    {
        // Simple slug generation - you might want to make this more sophisticated
        return title.ToLower()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("'", "")
            .Replace("\"", "");
    }
}