using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Web.Controllers
{
    public class SlideController : Controller
    {
        private readonly ISlideService _slideService;

        public SlideController(ISlideService slideService)
        {
            _slideService = slideService;
        }

        public async Task<IActionResult> Index(int? categoryId = null, string searchTerm = null, int pageIndex = 1)
        {
            var slides = await _slideService.GetSlidesAsync(pageIndex, categoryId, searchTerm);
            var categories = await _slideService.GetAllCategoriesAsync();

            var viewModel = new ListSlideDTO
            {
                Pagination = slides,
                DocumentCategories = categories
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("Slide/Detail/{id:int}/{slug?}")]
        public async Task<IActionResult> Detail(int id, string slug = null)
        {
            var slide = await _slideService.GetSlideDetailAsync(id);
            if (slide == null)
            {
                return NotFound();
            }

            // If a slug was provided but doesn't match the slide's slug, redirect to the correct URL
            if (slug != null && slug != slide.Slug)
            {
                return RedirectToAction(nameof(Detail), new { id = id, slug = slide.Slug });
            }

            // Set SEO metadata
            ViewData["Title"] = slide.Title;
            ViewData["Description"] = !string.IsNullOrEmpty(slide.Description) 
                ? slide.Description 
                : $"اسلاید {slide.Title} در دسته‌بندی {slide.DocumentCategory?.Name}";
            ViewData["Keywords"] = $"{slide.Title}, {slide.DocumentCategory?.Name}, {slide.DocumentType?.Name}, اسلاید, پاورپوینت, ارائه";
            ViewData["OgImage"] = !string.IsNullOrEmpty(slide.Picture) 
                ? $"{Request.Scheme}://{Request.Host}{slide.Picture}" 
                : $"{Request.Scheme}://{Request.Host}/assets/images/default-slide.jpg";

            await _slideService.IncrementViewCountAsync(id);
            return View(slide);
        }

        [Authorize]
        public async Task<IActionResult> MySlides(int pageIndex = 1)
        {
            const int pageSize = 12;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            var slides = await _slideService.GetUserSlidesAsync(userId, pageIndex, pageSize);
            var categories = await _slideService.GetAllCategoriesAsync();

            var viewModel = new ListSlideDTO
            {
                Pagination = slides,
                DocumentCategories = categories
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Author(string userId)
        {
            var authorName = await _slideService.GetAuthorNameAsync(userId);
            var authorSlides = await _slideService.GetAuthorSlidesAsync(userId);

            var viewModel = new AuthorSlideDTO
            {
                AuthorName = authorName,
                Slides = authorSlides
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Thumbnail(string filePath)
        {
            var stream = await _slideService.ConvertSlideToImageAsync(filePath);
            return File(stream, "image/jpeg");
        }
    }
}
