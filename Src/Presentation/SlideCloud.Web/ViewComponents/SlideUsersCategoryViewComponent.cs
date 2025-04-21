using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Web.ViewComponents
{
    public class SlideUsersCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public SlideUsersCategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var documentCategories = categories.Select(dto => new DocumentCategory
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Slug = dto.Slug
            });
            
            return View(documentCategories);
        }
    }
} 