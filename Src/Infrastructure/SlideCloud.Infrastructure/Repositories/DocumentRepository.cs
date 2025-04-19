using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Document> GetByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .Include(d => d.TagDocuments)
                .ThenInclude(td => td.Tag)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public IQueryable<Document> GetAll()
    {
        return _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .Include(d => d.TagDocuments)
                .ThenInclude(td => td.Tag);
    }

    public IQueryable<Document> Find(Expression<Func<Document, bool>> expression)
    {
        return _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .Include(d => d.TagDocuments)
                .ThenInclude(td => td.Tag)
            .Where(expression);
    }

    public async Task AddAsync(Document entity)
    {
        await _context.Documents.AddAsync(entity);
    }

    public void Update(Document entity)
    {
        _context.Documents.Update(entity);
    }

    public void Delete(Document entity)
    {
        _context.Documents.Remove(entity);
    }

    public async Task<IEnumerable<Document>> GetDocumentsByUserIdAsync(string userId)
    {
        var longUserId = long.Parse(userId);
        return await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentCategory)
            .Include(d => d.TagDocuments)
                .ThenInclude(td => td.Tag)
            .Where(d => d.UserId == longUserId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByCategoryIdAsync(int categoryId)
    {
        return await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .Where(d => d.DocumentCategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByTypeIdAsync(int typeId)
    {
        return await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .Include(d => d.TagDocuments)
                .ThenInclude(td => td.Tag)
            .Where(d => d.DocumentTypeId == typeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm)
    {
        return await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .Include(d => d.TagDocuments)
                .ThenInclude(td => td.Tag)
            .Where(d => d.Title.Contains(searchTerm) || d.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document != null)
        {
            document.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}