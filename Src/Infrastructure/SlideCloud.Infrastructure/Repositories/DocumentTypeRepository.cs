using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Infrastructure.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly AppDbContext _context;

    public DocumentTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentType> GetByIdAsync(int id)
    {
        return await _context.DocumentTypes
            .Include(dt => dt.Documents)
            .FirstOrDefaultAsync(dt => dt.Id == id);
    }

    public IQueryable<DocumentType> GetAll()
    {
        return _context.DocumentTypes
            .Include(dt => dt.Documents);
    }

    public IQueryable<DocumentType> Find(Expression<Func<DocumentType, bool>> expression)
    {
        return _context.DocumentTypes
            .Include(dt => dt.Documents)
            .Where(expression);
    }

    public async Task AddAsync(DocumentType entity)
    {
        await _context.DocumentTypes.AddAsync(entity);
    }

    public void Update(DocumentType entity)
    {
        _context.DocumentTypes.Update(entity);
    }

    public void Delete(DocumentType entity)
    {
        _context.DocumentTypes.Remove(entity);
    }

    public async Task<IEnumerable<DocumentType>> GetActiveDocumentTypesAsync()
    {
        return await _context.DocumentTypes
            .Include(dt => dt.Documents)
            .Where(dt => dt.IsActive)
            .ToListAsync();
    }

    public async Task<DocumentType> GetDocumentTypeWithDocumentsAsync(int id)
    {
        return await _context.DocumentTypes
            .Include(dt => dt.Documents)
                .ThenInclude(d => d.User)
            .Include(dt => dt.Documents)
                .ThenInclude(d => d.DocumentCategory)
            .FirstOrDefaultAsync(dt => dt.Id == id);
    }
} 