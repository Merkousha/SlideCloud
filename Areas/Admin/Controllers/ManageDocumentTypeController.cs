using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Areas.Admin.Models.DocumentType;
using SlideCloud.Data;

namespace SlideCloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageDocumentTypeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AppDbContext _appDbContext;

        public ManageDocumentTypeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }

        #region Create List


        public IActionResult List()
        {

            return View(_appDbContext.DocumentTypes.ToList());
        }

        #endregion


        #region Create DocumentType


        [HttpGet]
        public IActionResult CreateDocumentType()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocumentType(CreateDocumentTypeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _appDbContext.DocumentTypes.AddAsync(new SlideCloud.Models.DocumentType
            {
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug
            });
            _appDbContext.SaveChanges();
            return Redirect("/Admin/ManageDocumentType/List");
        }

        #endregion

        #region Create Update

        [HttpGet]
        public async Task<IActionResult> UpdateDocumentType(int Id)
        {
            var DocumentType = await _appDbContext.DocumentTypes.FindAsync(Id);
            if (DocumentType == null)
                return NotFound();
            var model = new UpdateDocumentTypeVM
            {
                Id=DocumentType.Id,
                Name = DocumentType.Name,
                Description = DocumentType.Description,
                Slug = DocumentType.Slug
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocumentType(UpdateDocumentTypeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var DocumentType = await _appDbContext.DocumentTypes.FindAsync(model.Id);
            if (DocumentType == null)
                return NotFound();


            DocumentType.Name = model.Name;
            DocumentType.Description = model.Description;
            DocumentType.Slug = model.Slug;

            _appDbContext.DocumentTypes.Update(DocumentType);
            _appDbContext.SaveChanges();
            return Redirect("/Admin/ManageDocumentType/List");
        }

        #endregion

        #region Delete DocumentType
        [HttpGet]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
          
            var DocumentType = await _appDbContext.DocumentTypes.FindAsync(id);
            if (DocumentType == null)
                return NotFound();

            _appDbContext.DocumentTypes.Remove(DocumentType);
            _appDbContext.SaveChanges();
            return Redirect("/Admin/ManageDocumentType/List");
        }

        #endregion
    }
}
