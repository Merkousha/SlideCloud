using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.DTO.DocumentType;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Application.Services
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentTypeDTO>> GetAllDocumentTypesAsync()
        {
            var types = await _unitOfWork.DocumentTypes.GetAll().ToListAsync();
            return types.Select(t => new DocumentTypeDTO
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Slug = t.Slug
            });
        }

        public async Task<DocumentTypeDTO> GetDocumentTypeByIdAsync(int id)
        {
            var type = await _unitOfWork.DocumentTypes.GetByIdAsync(id);
            if (type == null) return null;

            return new DocumentTypeDTO
            {
                Id = type.Id,
                Name = type.Name,
                Description = type.Description,
                Slug = type.Slug
            };
        }

        public async Task<int> CreateDocumentTypeAsync(DocumentTypeDTO typeDto)
        {
            var type = new DocumentType
            {
                Name = typeDto.Name,
                Description = typeDto.Description,
                Slug = typeDto.Slug
            };

            await _unitOfWork.DocumentTypes.AddAsync(type);
            await _unitOfWork.SaveChangesAsync();
            return type.Id;
        }

        public async Task UpdateDocumentTypeAsync(DocumentTypeDTO typeDto)
        {
            var type = await _unitOfWork.DocumentTypes.GetByIdAsync(typeDto.Id);
            if (type != null)
            {
                type.Name = typeDto.Name;
                type.Description = typeDto.Description;
                type.Slug = typeDto.Slug;

                _unitOfWork.DocumentTypes.Update(type);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteDocumentTypeAsync(int id)
        {
            var type = await _unitOfWork.DocumentTypes.GetByIdAsync(id);
            if (type != null)
            {
                _unitOfWork.DocumentTypes.Delete(type);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> DocumentTypeExistsAsync(int id)
        {
            var type = await _unitOfWork.DocumentTypes.GetByIdAsync(id);
            return type != null;
        }
    }
} 