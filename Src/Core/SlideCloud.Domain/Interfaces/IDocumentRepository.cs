using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Entities;
using System.Linq.Expressions;

namespace SlideCloud.Domain.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetDocumentsByUserIdAsync(string userId);
    Task<IEnumerable<Document>> GetDocumentsByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Document>> GetDocumentsByTypeIdAsync(int typeId);
    Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm);
    Task IncrementViewCountAsync(int id);
    IQueryable<Document> GetAll();
}