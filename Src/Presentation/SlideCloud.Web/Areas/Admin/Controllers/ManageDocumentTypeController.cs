using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.DocumentType;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageDocumentTypeController : Controller
    {
        private readonly IDocumentTypeService _documentTypeService;

        public ManageDocumentTypeController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        #region List Document Types
        public async Task<IActionResult> List()
        {
            var documentTypeDtos = await _documentTypeService.GetAllDocumentTypesAsync();
            var documentTypes = documentTypeDtos.Select(dto => new DocumentType
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Slug = dto.Slug
            });
            return View(documentTypes);
        }
        #endregion

        #region Create Document Type
        [HttpGet]
        public IActionResult CreateDocumentType()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocumentType(DocumentTypeDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _documentTypeService.CreateDocumentTypeAsync(model);
            return RedirectToAction(nameof(List));
        }
        #endregion

        #region Update Document Type
        [HttpGet]
        public async Task<IActionResult> UpdateDocumentType(int id)
        {
            var documentType = await _documentTypeService.GetDocumentTypeByIdAsync(id);
            if (documentType == null)
            {
                return NotFound();
            }

            return View(documentType);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocumentType(DocumentTypeDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var documentType = await _documentTypeService.GetDocumentTypeByIdAsync(model.Id);
            if (documentType == null)
            {
                return NotFound();
            }

            await _documentTypeService.UpdateDocumentTypeAsync(model);
            return RedirectToAction(nameof(List));
        }
        #endregion

        #region Delete Document Type
        [HttpGet]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            if (!await _documentTypeService.DocumentTypeExistsAsync(id))
            {
                return NotFound();
            }

            await _documentTypeService.DeleteDocumentTypeAsync(id);
            return RedirectToAction(nameof(List));
        }
        #endregion
    }
}
