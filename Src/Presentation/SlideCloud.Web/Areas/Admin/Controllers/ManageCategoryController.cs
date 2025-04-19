using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Category;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public ManageCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        #region List Categories
        public async Task<IActionResult> List()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
        #endregion

        #region Create Category
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _categoryService.CreateCategoryAsync(model);
            return RedirectToAction(nameof(List));
        }
        #endregion

        #region Update Category
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(CategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await _categoryService.GetCategoryByIdAsync(model.Id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryService.UpdateCategoryAsync(model);
            return RedirectToAction(nameof(List));
        }
        #endregion

        #region Delete Category
        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!await _categoryService.CategoryExistsAsync(id))
            {
                return NotFound();
            }

            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(List));
        }
        #endregion
    }
}
