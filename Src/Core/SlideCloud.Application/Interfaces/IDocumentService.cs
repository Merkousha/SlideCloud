using System.Collections.Generic;
using System.Threading.Tasks;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Application.Interfaces;

public interface IDocumentService
{
    Task<Document> GetDocumentByIdAsync(int id);
    Task<IEnumerable<Document>> GetAllDocumentsAsync();
    Task<IEnumerable<Document>> GetUserDocumentsAsync(string userId);
    Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm);
    Task<Document> CreateDocumentAsync(Document document);
    Task UpdateDocumentAsync(Document document);
    Task DeleteDocumentAsync(int id);
    Task IncrementViewCountAsync(int documentId);
} 