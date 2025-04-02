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
        public IActionResult List()
        {

            return View(_appDbContext.DocumentCategories.ToList());
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {

            return View();
        }

        [HttpPost]
        public async Task< IActionResult> CreateCategory(CreateCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
         await   _appDbContext.DocumentCategories.AddAsync(new SlideCloud.Models.DocumentCategory
            {
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug
            });
            _appDbContext.SaveChanges();
            return Redirect("/Admin/ManageCategory/List");
        }
    }
}
