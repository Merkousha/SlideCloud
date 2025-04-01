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
            

            _appDbContext.Documents.Add(new Document
            {
                Title = model.Title,
                Description = model.Description,
                File = model.File,
                Picture = model.Picture,
                DocumentTypeId = model.DocumentTypeId,
                DocumentCategoryId = model.DocumentCategoryId,
                ViewCount = 0
            });

            await _appDbContext.SaveChangesAsync();
            return Redirect("~/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSlide(IFormFile file, string type)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "فایلی دریافت نشد" });

                // بررسی نوع فایل
                if (type == "picture" && !file.ContentType.StartsWith("image/"))
                    return BadRequest(new { message = "فایل انتخاب شده باید تصویر باشد" });

                if (type == "file" && !IsValidSlideFile(file))
                    return BadRequest(new { message = "فایل باید از نوع PDF یا PowerPoint باشد" });

                var fileUrl = await _s3Uploader.UploadFileAsync(file);
                return Ok(new { filePath = fileUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "خطا در آپلود فایل: " + ex.Message });
            }
        }

        private bool IsValidSlideFile(IFormFile file)
        {
            var validExtensions = new[] { ".pdf", ".ppt", ".pptx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return validExtensions.Contains(extension);
        }



    }
}
