using System.Drawing.Printing;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Areas.Admin.Models.Slides;
using SlideCloud.Data;
using SlideCloud.Models;


namespace SlideCloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageSlideController : Microsoft.AspNetCore.Mvc.Controller
    {
        UserManager<SlideCloud.Models.User> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly IS3Uploader _s3Uploader;

        public ManageSlideController(AppDbContext appDbContext, IConfiguration configuration, IS3Uploader s3Uploader, UserManager<SlideCloud.Models.User> userManager)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
            _s3Uploader = s3Uploader;
            _userManager = userManager;
        }

        #region List Of Sldie For Admin
        public async Task<IActionResult> ListOfSlide()
        {
            var query = _appDbContext.Documents.OrderBy(u => u.Id).ToList();
            return View(query);
        }
        #endregion


        #region ManageSlidesForAdmin

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

            var user = await _userManager.Users.Where(a => a.Email.Equals(User.Identity.Name)).FirstOrDefaultAsync();
            _appDbContext.Documents.Add(new Document
            {
                Title = model.Title,
                Description = model.Description,
                File = model.File,
                Picture = model.Picture,
                DocumentTypeId = model.DocumentTypeId,
                DocumentCategoryId = model.DocumentCategoryId,
                UserId = user.Id,
                ViewCount = 0
            });

            await _appDbContext.SaveChangesAsync();
            return Redirect("/Admin/ManageSlide/ListOfSlide");
           // return RedirectToAction("ListOfSlide", "ManageSlide");
            
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



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

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

            if (!ModelState.IsValid)
                return View(model);

            var documnet = await _appDbContext.Documents.FindAsync(model.Id);

            var user = await _userManager.Users.Where(a => a.Email.Equals(User.Identity.Name)).FirstOrDefaultAsync();
            if (user != null)
            {
                if (documnet != null)
                {
                    documnet.Description = model.Description;
                    documnet.DocumentCategoryId = model.DocumentCategoryId;
                    documnet.DocumentTypeId = model.DocumentTypeId;
                    documnet.File = model.File;
                    documnet.Picture = model.Picture;
                    documnet.Title = model.Title;
                    documnet.UserId = user.Id;
                    _appDbContext.Documents.Update(documnet);
                    await _appDbContext.SaveChangesAsync();

                    return RedirectToAction("ListOfSlide", "ManageSlide", new { id = model.Id });
                }

            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var Document = await _appDbContext.Documents.FirstOrDefaultAsync(a => a.Id == id);

            if (Document == null)
            {
                return NotFound();
            }

                _appDbContext.Documents.Remove(Document);
                await _appDbContext.SaveChangesAsync();
                return Redirect("~/Admin/ManageSlide/ListOfSlide");
       

        }

        #endregion



        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<long, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }



        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.AddToRoleAsync(user, "Admin");

            string url = "/Admin/ManageSlide/Index";

            return Redirect(url);
        }

        public async Task<IActionResult> DemoteToUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, "Admin");

            string url = "/Admin/ManageSlide/Index";
            return Redirect(url);
        }

    }
}
