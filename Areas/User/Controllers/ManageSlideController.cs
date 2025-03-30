using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Areas.User.Models.Slides;
using SlideCloud.Data;
using SlideCloud.Models;

namespace SlideCloud.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ManageSlideController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AppDbContext _appDbContext;

        public ManageSlideController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }


        [HttpGet]
        public IActionResult Create()
        {

            var model = new CreateSlides();
            ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateSlides model)
        {
            ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();
            if (!ModelState.IsValid)
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

            //return  RedirectToAction("Index", "Home");
            return Redirect("~/");
        }




        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
         
            //check if the user is the owner of the document
            //if()
            var documnet = await _appDbContext.Documents.FindAsync(id);
            if (documnet != null)
            {
                var model = new UpdateSlides();

                model.Id = documnet.Id;
                model.Description = documnet.Description;
                model.DocumentCategoryId = documnet.DocumentCategoryId;
                model.DocumentTypeId = documnet.DocumentTypeId;
                model.File = documnet.File;
                model.Picture = documnet.Picture;
                model.Title = documnet.Title;
                model.ViewCount = model.ViewCount;
                ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
                ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();
                return View(model);
            }

            return NotFound();


        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateSlides model)
        {
            ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();
            //check if the user is the owner of the document
            //if()

            if (!ModelState.IsValid)
                return View(model);

            var documnet = await _appDbContext.Documents.FindAsync(model.Id);
            if (documnet != null)
            {

                documnet.Description = model.Description;
                documnet.DocumentCategoryId = model.DocumentCategoryId;
                documnet.DocumentTypeId = model.DocumentTypeId;
                documnet.File = model.File;
                documnet.Picture = model.Picture;
                documnet.Title = model.Title;
                _appDbContext.Documents.Update(documnet);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "Slide", new { id = model.Id });
            }
            return View(model);

        }


    }
}
