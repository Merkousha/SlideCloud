using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Slide;
using SlideCloud.Application.Interfaces;

namespace SlideCloud.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ManageSlideController : Controller
    {
        private readonly ISlideService _slideService;
        private readonly IFileService _fileService;
        private readonly ICategoryService _categoryService;
        private readonly IDocumentTypeService _typeService;
        private readonly IUserService _userService;

        public ManageSlideController(
            ISlideService slideService,
            IFileService fileService,
            ICategoryService categoryService,
            IDocumentTypeService typeService,
             IUserService userService)
        {
            _slideService = slideService;
            _fileService = fileService;
            _categoryService = categoryService;
            _typeService = typeService;
            _userService = userService;
        }

        #region Create Slide
        [HttpGet]
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
        public async Task<IActionResult> Create(DetailsSlideDTO model)
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

            if (model.NewFile == null)
            {
                ModelState.AddModelError("NewFile", "Please select a file to upload.");
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }

            if (!_fileService.IsValidFileType(model.NewFile, new[] { ".pdf", ".ppt", ".pptx" }))
            {
                ModelState.AddModelError("NewFile", "Invalid file type. Only PDF and PowerPoint files are allowed.");
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
                return View(model);
            }

            try
            {
                var filePath = await _fileService.UploadFileAsync(model.NewFile);
                var picturePath = model.NewPicture != null ? await _fileService.UploadFileAsync(model.NewPicture) : null;

                await _slideService.CreateSlideAsync(new SlideCreateDTO
                {
                    Title = model.Title,
                    Description = model.Description,
                    File = filePath,
                    Picture = picturePath,
                    DocumentTypeId = model.DocumentTypeId,
                    DocumentCategoryId = model.DocumentCategoryId,
                    UserId = user.Id.ToString()
                });

                return RedirectToAction("MySlides", "Slide");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the slide. Please try again.");
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                model.Types = await _typeService.GetAllDocumentTypesAsync();
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

            slide.Categories = await _categoryService.GetAllCategoriesAsync();
            slide.Types = await _typeService.GetAllDocumentTypesAsync();

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
                    filePath = await _fileService.UploadFileAsync(model.NewFile);
                }

                if (model.NewPicture != null)
                {
                    if (slide.Picture != null)
                    {
                        await _fileService.DeleteFileAsync(slide.Picture);
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

                return RedirectToAction("MySlides", "Slide");
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
