using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ManageSlideController : Controller
    {
        private readonly ISlideService _slideService;
        private readonly IS3Uploader _s3Uploader;
        private readonly IFileService _fileService;
        private readonly ICategoryService _categoryService;
        private readonly IDocumentTypeService _typeService;
        private readonly IUserService _userService;

        public ManageSlideController(
            ISlideService slideService,
            IS3Uploader s3Uploader,
            ICategoryService categoryService,
            IDocumentTypeService typeService,
             IUserService userService,
             IFileService fileService)
        {
            _slideService = slideService;
            _s3Uploader = s3Uploader;
            _categoryService = categoryService;
            _typeService = typeService;
            _userService = userService;
            _fileService = fileService;
        }

        #region Create Slide
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new DetailsSlideDTO();
            var types = await _typeService.GetAllDocumentTypesAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();

            ViewBag.DocumentTypes = types;
            ViewBag.DocumentCategories = categories;

            // Add empty item to dropdowns
            var documentTypeList = types.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()
            }).ToList();
            documentTypeList.Insert(0, new SelectListItem { Text = "انتخاب کنید...", Value = "" });

            var documentCategoryList = categories.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()
            }).ToList();
            documentCategoryList.Insert(0, new SelectListItem { Text = "انتخاب کنید...", Value = "" });

            ViewBag.DocumentTypeList = documentTypeList;
            ViewBag.DocumentCategoryList = documentCategoryList;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadSlide(IFormFile file, string type)
        {
            if (file == null)
            {
                return Json(new { error = "No file was uploaded." });
            }

            string[] allowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif" };
            string[] allowedDocumentTypes = { ".pdf", ".ppt", ".pptx" };

            if (type == "picture" && !_fileService.IsValidFileType(file, allowedImageTypes))
            {
                return Json(new { error = "Invalid image type. Only JPG, JPEG, PNG and GIF files are allowed." });
            }

            if (type == "file" && !_fileService.IsValidFileType(file, allowedDocumentTypes))
            {
                return Json(new { error = "Invalid file type. Only PDF and PowerPoint files are allowed." });
            }

            try
            {
                var filePath = await _s3Uploader.UploadFileAsync(file);
                return Json(new { filePath = filePath });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred while uploading the file." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(DetailsSlideDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DocumentTypes = await _typeService.GetAllDocumentTypesAsync();
                ViewBag.DocumentCategories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                return NotFound();
            }

            try
            {

                await _slideService.CreateSlideAsync(new SlideCreateDTO
                {
                    Title = model.Title,
                    Description = model.Description,
                    File = model.File,
                    Picture = model.Picture,
                    DocumentTypeId = model.DocumentTypeId,
                    DocumentCategoryId = model.DocumentCategoryId,
                    UserId = user.Id.ToString()
                });

                return RedirectToAction("MySlides", "Slide", new { area = "" });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the slide. Please try again.");
                ViewBag.DocumentTypes = await _typeService.GetAllDocumentTypesAsync();
                ViewBag.DocumentCategories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }
        }
        #endregion

        #region Update Slide
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                return NotFound();
            }

            var slide = await _slideService.GetSlideByIdAsync(id);
            if (slide == null || slide.UserId != user.Id.ToString())
            {
                return NotFound();
            }

            var types = await _typeService.GetAllDocumentTypesAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();

            // Add empty item to dropdowns
            var documentTypeList = types.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString(),
                Selected = a.Id == slide.DocumentTypeId
            }).ToList();
            documentTypeList.Insert(0, new SelectListItem { Text = "انتخاب کنید...", Value = "" });

            var documentCategoryList = categories.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString(),
                Selected = a.Id == slide.DocumentCategoryId
            }).ToList();
            documentCategoryList.Insert(0, new SelectListItem { Text = "انتخاب کنید...", Value = "" });

            ViewBag.DocumentTypeList = documentTypeList;
            ViewBag.DocumentCategoryList = documentCategoryList;

            return View(slide);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DetailsSlideDTO model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }

            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                return NotFound();
            }

            var slide = await _slideService.GetSlideByIdAsync(model.Id);
            if (slide == null || slide.UserId != user.Id.ToString())
            {
                return NotFound();
            }

            try
            {
                string filePath = slide.File;
                string picturePath = slide.Picture;

                if (model.NewFile != null)
                {
                    if (!_fileService.IsValidFileType(model.NewFile, new[] { ".pdf", ".ppt", ".pptx" }))
                    {
                        ModelState.AddModelError("NewFile", "Invalid file type. Only PDF and PowerPoint files are allowed.");
                        model.Categories = await _categoryService.GetAllCategoriesAsync();
                        model.Types = await _typeService.GetAllDocumentTypesAsync();
                        return View(model);
                    }

                    await _fileService.DeleteFileAsync(slide.File);
                    filePath = await _s3Uploader.UploadFileAsync(model.NewFile);
                }

                if (model.NewPicture != null)
                {
                    if (slide.Picture != null)
                    {
                        await _fileService.DeleteFileAsync(slide.Picture);
                    }
                    picturePath = await _s3Uploader.UploadFileAsync(model.NewPicture);
                }

                await _slideService.UpdateSlideAsync(new SlideUpdateDTO
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    File = filePath,
                    Picture = picturePath,
                    DocumentTypeId = model.DocumentTypeId,
                    DocumentCategoryId = model.DocumentCategoryId
                });

                return RedirectToAction("MySlides", "Slide", new { area = "" });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating the slide. Please try again.");
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }
        }
        #endregion

        #region Delete Slide
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                return NotFound();
            }

            var slide = await _slideService.GetSlideByIdAsync(id);
            if (slide == null || slide.UserId != user.Id.ToString())
            {
                return NotFound();
            }

            await _slideService.DeleteSlideAsync(id);
            return RedirectToAction("MySlides", "Slide");
        }
        #endregion
    }
}
