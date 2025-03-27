using Microsoft.AspNetCore.Mvc;
using SlideCloud.Areas.Admin.Models.Slides;
using SlideCloud.Data;

namespace SlideCloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageSlideController : Microsoft.AspNetCore.Mvc.Controller
    {
        AppDbContext _appDbContext;

        public ManageSlideController(AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;

        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateSlides();
            model.DocumentTypes= _appDbContext.DocumentTypes.ToList();
            model.DocumentCategories= _appDbContext.DocumentCategories.ToList();
            return View(model);
        }
        [HttpPost]


        public IActionResult Create(CreateSlides model)
        {
            model.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            model.DocumentCategories = _appDbContext.DocumentCategories.ToList();
            if (ModelState.ErrorCount>2)
            {
                
                return View(model);

            }
            _appDbContext.Documents.Add(new SlideCloud.Models.Document
            {
                Description = model.Description,
                DocumentCategoryId = model.DocumentCategoryId,
                DocumentTypeId = model.DocumentTypeId,
                File = model.File,
                Picture = model.Picture,
                Title = model.Title,
                ViewCount = 0

            });
            _appDbContext.SaveChanges();
            return Redirect("/");
        }




    }
}
