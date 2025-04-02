using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Areas.Admin.Models.Category;
using SlideCloud.Data;

namespace SlideCloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageCategoryController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AppDbContext _appDbContext;

        public ManageCategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }

        #region Create List


        public IActionResult List()
        {

            return View(_appDbContext.DocumentCategories.ToList());
        }

        #endregion


        #region Create Category


        [HttpGet]
        public IActionResult CreateCategory()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _appDbContext.DocumentCategories.AddAsync(new SlideCloud.Models.DocumentCategory
            {
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug
            });
            _appDbContext.SaveChanges();
            return Redirect("/Admin/ManageCategory/List");
        }

        #endregion

        #region Create Update

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int Id)
        {
            var category = await _appDbContext.DocumentCategories.FindAsync(Id);
            if (category == null)
                return NotFound();
            var model = new UpdateCategoryVM
            {
                Id=category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var category = await _appDbContext.DocumentCategories.FindAsync(model.Id);
            if (category == null)
                return NotFound();


            category.Name = model.Name;
            category.Description = model.Description;
            category.Slug = model.Slug;

            _appDbContext.DocumentCategories.Update(category);
            _appDbContext.SaveChanges();
            return Redirect("/Admin/ManageCategory/List");
        }

        #endregion

        #region Delete Category
        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
          
            var category = await _appDbContext.DocumentCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            _appDbContext.DocumentCategories.Remove(category);
            _appDbContext.SaveChanges();
            return Redirect("/Admin/ManageCategory/List");
        }

        #endregion
    }
}
