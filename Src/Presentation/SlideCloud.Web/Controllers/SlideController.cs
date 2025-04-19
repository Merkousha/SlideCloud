using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.Pagination;
using SlideCloud.Web.Models;
using System.Security.Claims;

namespace SlideCloud.Web.Controllers
{
    public class SlideController : Controller
    {
        private const int PageSize = 15;
        private readonly UserManager<User> _userManager;
        private readonly ISlideService _slideService;

        public SlideController(UserManager<User> userManager, ISlideService slideService)
        {
            _userManager = userManager;
            _slideService = slideService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int? categoryId = null)
        {
            const int pageSize = 12;
            var slides = await _slideService.GetSlidesAsync(pageIndex, categoryId, pageSize);

            var viewModel = new SlideIndexViewModel
            {
                Pagination = slides,
                Categories = await _slideService.GetAllCategoriesAsync(),
                SelectedCategoryId = categoryId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var slide = await _slideService.GetSlideDetailAsync(id);
            if (slide == null)
            {
                return NotFound();
            }

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

            var viewModel = new SlideIndexViewModel
            {
                Pagination = slides,
                Categories = await _slideService.GetAllCategoriesAsync()
            };

            return View("Index", viewModel);
        }

        public async Task<IActionResult> ListSlide_Category(int pageIndex = 1, int? categoryId = null)
        {
            var pagination = await _slideService.GetSlidesAsync(pageIndex, categoryId, PageSize);
            var categories = await _slideService.GetAllCategoriesAsync();

            var model = new ListSlideVM
            {
                Pagination = pagination,
                DocumentCategories = categories,
                SelectedCategoryId = categoryId
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Author(string userId)
        {
            var authorName = await _slideService.GetAuthorNameAsync(userId);
            var authorSlides = await _slideService.GetAuthorSlidesAsync(userId);

            var viewModel = new AuthorViewModel
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
