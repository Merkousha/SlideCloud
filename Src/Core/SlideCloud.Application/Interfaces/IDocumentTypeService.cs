using SlideCloud.Application.DTO.DocumentType;

namespace SlideCloud.Application.Interfaces
{
    public interface IDocumentTypeService
    {
        Task<IEnumerable<DocumentTypeDTO>> GetAllDocumentTypesAsync();
        Task<DocumentTypeDTO> GetDocumentTypeByIdAsync(int id);
        Task<int> CreateDocumentTypeAsync(DocumentTypeDTO documentType);
        Task UpdateDocumentTypeAsync(DocumentTypeDTO documentType);
        Task DeleteDocumentTypeAsync(int id);
        Task<bool> DocumentTypeExistsAsync(int id);
    }
} 