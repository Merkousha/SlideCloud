using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.Interfaces;
using SlideCloud.Web.Areas.Admin.Models.Slides;

namespace SlideCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SlideController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ISlideService _slideService;
        private readonly IFileService _fileService;
        private readonly ICategoryService _categoryService;
        private readonly IDocumentTypeService _typeService;

        public SlideController(
            ISlideService slideService,
            IFileService fileService,
            ICategoryService categoryService,
            IDocumentTypeService typeService)
        {
            _slideService = slideService;
            _fileService = fileService;
            _categoryService = categoryService;
            _typeService = typeService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int? categoryId = null, int pageSize = 10)
        {
            var documents = await _slideService.GetSlidesAsync(pageIndex, categoryId, null);
            var categories = await _categoryService.GetAllCategoriesAsync();

            var model = new ListSlideVM
            {
                Documents = documents,
                Categories = categories,
                SelectedCategoryId = categoryId
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new DetailsSlideDTO
            {
                Categories = await _categoryService.GetAllCategoriesAsync(),
                Types = await _typeService.GetAllDocumentTypesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetailsSlideDTO model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }

            if (model.NewFile == null)
            {
                ModelState.AddModelError("NewFile", "File is required");
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }

            if (!_fileService.IsValidFileType(model.NewFile, new[] { ".ppt", ".pptx", ".pdf" }))
            {
                ModelState.AddModelError("NewFile", "فرمت فایل نامعتبر است");
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }

            var filePath = await _fileService.UploadFileAsync(model.NewFile);
            var picturePath = model.NewPicture != null ? await _fileService.UploadFileAsync(model.NewPicture) : null;

            await _slideService.CreateSlideAsync(new SlideCreateDTO
            {
                Title = model.Title,
                Description = model.Description,
                File = filePath,
                Picture = picturePath,
                DocumentCategoryId = model.DocumentCategoryId,
                DocumentTypeId = model.DocumentTypeId,
                UserId = User.Identity.Name
            });

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var document = await _slideService.GetSlideByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var model = new DetailsSlideDTO
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                DocumentCategoryId = document.DocumentCategoryId,
                DocumentTypeId = document.DocumentTypeId,
                File = document.File,
                Picture = document.Picture,
                Categories = await _categoryService.GetAllCategoriesAsync(),
                Types = await _typeService.GetAllDocumentTypesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DetailsSlideDTO model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }

            var document = await _slideService.GetSlideByIdAsync(model.Id);
            if (document == null)
            {
                return NotFound();
            }

            string filePath = document.File;
            string picturePath = document.Picture;

            if (model.NewFile != null)
            {
                if (!_fileService.IsValidFileType(model.NewFile, new[] { ".ppt", ".pptx", ".pdf" }))
                {
                    ModelState.AddModelError("NewFile", "فرمت فایل نامعتبر است");
                    model.Categories = await _categoryService.GetAllCategoriesAsync();
                    model.Types = await _typeService.GetAllDocumentTypesAsync();
                    return View(model);
                }

                await _fileService.DeleteFileAsync(document.File);
                filePath = await _fileService.UploadFileAsync(model.NewFile);
            }

            if (model.NewPicture != null)
            {
                if (document.Picture != null)
                {
                    await _fileService.DeleteFileAsync(document.Picture);
                }
                picturePath = await _fileService.UploadFileAsync(model.NewPicture);
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _slideService.GetSlideByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            if (document.File != null)
            {
                await _fileService.DeleteFileAsync(document.File);
            }

            if (document.Picture != null)
            {
                await _fileService.DeleteFileAsync(document.Picture);
            }

            await _slideService.DeleteSlideAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}