using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageSlideController : Controller
    {
        private readonly IAdminSlideService _slideService;
        private readonly IUserService _userService;

        public ManageSlideController(IAdminSlideService slideService, IUserService userService)
        {
            _slideService = slideService;
            _userService = userService;
        }

        #region List Slides
        public async Task<IActionResult> ListOfSlide()
        {
            var slides = await _slideService.GetAllSlidesAsync();
            return View(slides);
        }
        #endregion

        #region Create Slide
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.DocumentTypes = await _slideService.GetAllDocumentTypesAsync();
            ViewBag.DocumentCategories = await _slideService.GetAllCategoriesAsync();

            return View(new DetailsSlideDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(DetailsSlideDTO model)
        {
            ViewBag.DocumentTypes = await _slideService.GetAllDocumentTypesAsync();
            ViewBag.DocumentCategories = await _slideService.GetAllCategoriesAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                return NotFound();
            }

            var document = new Document
            {
                Title = model.Title,
                Description = model.Description,
                File = model.File,
                Picture = model.Picture,
                DocumentTypeId = model.DocumentTypeId,
                DocumentCategoryId = model.DocumentCategoryId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ViewCount = 0,
                IsShared = false
            };

            await _slideService.CreateSlideAsync(document);

            return RedirectToAction(nameof(ListOfSlide));
        }
        #endregion

        #region Upload File
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSlide(IFormFile file, string type)
        {
            try
            {
                var fileUrl = await _slideService.UploadFileAsync(file, type);
                return Ok(new { filePath = fileUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error uploading file: " + ex.Message });
            }
        }
        #endregion

        #region Update Slide
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var slide = await _slideService.GetSlideByIdAsync(id);
            if (slide == null)
            {
                return NotFound();
            }

            ViewBag.DocumentTypes = await _slideService.GetAllDocumentTypesAsync();
            ViewBag.DocumentCategories = await _slideService.GetAllCategoriesAsync();

            var model = new DetailsSlideDTO
            {
                Id = slide.Id,
                Title = slide.Title,
                Description = slide.Description,
                File = slide.File,
                Picture = slide.Picture,
                DocumentTypeId = slide.DocumentTypeId,
                DocumentCategoryId = slide.DocumentCategoryId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DetailsSlideDTO model)
        {
            ViewBag.DocumentTypes = await _slideService.GetAllDocumentTypesAsync();
            ViewBag.DocumentCategories = await _slideService.GetAllCategoriesAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var document = await _slideService.GetSlideByIdAsync(model.Id);
            if (document == null)
            {
                return NotFound();
            }

            document.Title = model.Title;
            document.Description = model.Description;
            document.File = model.File;
            document.Picture = model.Picture;
            document.DocumentTypeId = model.DocumentTypeId;
            document.DocumentCategoryId = model.DocumentCategoryId;
            document.UpdatedAt = DateTime.UtcNow;

            await _slideService.UpdateSlideAsync(document);

            return RedirectToAction(nameof(ListOfSlide));
        }
        #endregion

        #region Delete Slide
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _slideService.SlideExistsAsync(id))
            {
                return NotFound();
            }

            await _slideService.DeleteSlideAsync(id);
            return RedirectToAction(nameof(ListOfSlide));
        }
        #endregion

        #region User Management
        public async Task<IActionResult> PromoteToAdmin(long id)
        {

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            //TODO: Fix tthis line
            //await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public async Task<IActionResult> DemoteToUser(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            //TODO: Fix tthis line
            //await _userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        #endregion
    }
}
