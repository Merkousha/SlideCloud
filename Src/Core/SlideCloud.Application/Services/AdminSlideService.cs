using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Application.Services
{
    public class AdminSlideService : IAdminSlideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IS3Uploader _s3Uploader;

        public AdminSlideService(IUnitOfWork unitOfWork, IS3Uploader s3Uploader)
        {
            _unitOfWork = unitOfWork;
            _s3Uploader = s3Uploader;
        }

        public async Task<IEnumerable<Document>> GetAllSlidesAsync()
        {
            return await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.DocumentType)
                .Include(d => d.User)
                .ToListAsync();
        }

        public async Task<Document> GetSlideByIdAsync(int id)
        {
            return await _unitOfWork.Documents.GetAll()
                .Include(d => d.DocumentCategory)
                .Include(d => d.DocumentType)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<int> CreateSlideAsync(Document document)
        {
            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();
            return document.Id;
        }

        public async Task UpdateSlideAsync(Document document)
        {
            var existingDocument = await _unitOfWork.Documents.GetByIdAsync(document.Id);
            if (existingDocument == null)
                throw new ArgumentException($"Document with ID {document.Id} not found");

            if (existingDocument.DocumentCategoryId != document.DocumentCategoryId)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(document.DocumentCategoryId);
                if (category == null)
                    throw new ArgumentException($"Category with ID {document.DocumentCategoryId} not found");
                document.DocumentCategory = category;
            }

            if (existingDocument.DocumentTypeId != document.DocumentTypeId)
            {
                var type = await _unitOfWork.DocumentTypes.GetByIdAsync(document.DocumentTypeId);
                if (type == null)
                    throw new ArgumentException($"Document type with ID {document.DocumentTypeId} not found");
                document.DocumentType = type;
            }

            document.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Documents.Update(document);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSlideAsync(int id)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            if (document != null)
            {
                _unitOfWork.Documents.Delete(document);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> SlideExistsAsync(int id)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            return document != null;
        }

        public async Task<IEnumerable<DocumentType>> GetAllDocumentTypesAsync()
        {
            return await Task.FromResult(_unitOfWork.DocumentTypes.GetAll().ToList());
        }

        public async Task<IEnumerable<DocumentCategory>> GetAllCategoriesAsync()
        {
            return await Task.FromResult(_unitOfWork.Categories.GetAll().ToList());
        }

        public async Task<string> UploadFileAsync(IFormFile file, string type)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file was received");

            // Validate file type
            if (type == "picture" && !file.ContentType.StartsWith("image/"))
                throw new ArgumentException("The selected file must be an image");

            if (type == "file" && !IsValidSlideFile(file))
                throw new ArgumentException("The file must be a PDF or PowerPoint file");

            return await _s3Uploader.UploadFileAsync(file);
        }

        private bool IsValidSlideFile(IFormFile file)
        {
            var validExtensions = new[] { ".pdf", ".ppt", ".pptx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return validExtensions.Contains(extension);
        }
    }
}