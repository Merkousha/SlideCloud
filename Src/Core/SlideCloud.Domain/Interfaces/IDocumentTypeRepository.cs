using SlideCloud.Domain.Entities;

namespace SlideCloud.Domain.Interfaces
{
    public interface IDocumentTypeRepository : IRepository<DocumentType>
    {
        Task<IEnumerable<DocumentType>> GetActiveDocumentTypesAsync();
        Task<DocumentType> GetDocumentTypeWithDocumentsAsync(int id);
    }
} 