using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Areas.User.Models.Slides;
using SlideCloud.Data;
using SlideCloud.Models;
using SlideCloud.Services;

namespace SlideCloud.Areas.User.Controllers
{

    [Area("User")]
    [Authorize]
    public class ManageSlideController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly IS3Uploader _s3Uploader;


        public ManageSlideController(AppDbContext appDbContext, IConfiguration configuration, IS3Uploader s3Uploader)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
            _s3Uploader = s3Uploader;
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();

            var model = new CreateSlides();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSlides model)
        {
            ViewBag.DocumentTypes = _appDbContext.DocumentTypes.ToList();
            ViewBag.DocumentCategories = _appDbContext.DocumentCategories.ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // upload file to S3
            var fileUrl = await _s3Uploader.UploadFileAsync(model.File);
            var pictureUrl = await _s3Uploader.UploadFileAsync(model.Picture);

            _appDbContext.Documents.Add(new Document
            {
                Title = model.Title,
                Description = model.Description,
                File = fileUrl,
                Picture = pictureUrl,
                DocumentTypeId = model.DocumentTypeId,
                DocumentCategoryId = model.DocumentCategoryId,
                ViewCount = 0
            });

            await _appDbContext.SaveChangesAsync();
            return Redirect("~/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSlide(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "فایلی دریافت نشد" });

            // استفاده از کلاس S3Uploader که قبلاً گفتیم
            var uploader = new S3Uploader(_configuration);
            var fileUrl = await uploader.UploadFileAsync(file); // برگردوندن URL کامل

            return Ok(new { fileUrl });
        }



    }
}
