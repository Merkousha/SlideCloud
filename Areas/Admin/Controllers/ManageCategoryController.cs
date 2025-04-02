using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
