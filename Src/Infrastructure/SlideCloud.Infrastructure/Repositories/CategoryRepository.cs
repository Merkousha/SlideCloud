using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentCategory> GetByIdAsync(int id)
    {
        return await _context.DocumentCategories
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public IQueryable<DocumentCategory> GetAll()
    {
        return _context.DocumentCategories
            .Include(c => c.Documents);
    }

    public IQueryable<DocumentCategory> Find(Expression<Func<DocumentCategory, bool>> expression)
    {
        return _context.DocumentCategories
            .Include(c => c.Documents)
            .Where(expression);
    }

    public async Task AddAsync(DocumentCategory entity)
    {
        await _context.DocumentCategories.AddAsync(entity);
    }

    public void Update(DocumentCategory entity)
    {
        _context.DocumentCategories.Update(entity);
    }

    public void Delete(DocumentCategory entity)
    {
        _context.DocumentCategories.Remove(entity);
    }

    public async Task<IEnumerable<DocumentCategory>> GetActiveCategoriesAsync()
    {
        return await _context.DocumentCategories
            .Include(c => c.Documents)
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<DocumentCategory> GetCategoryWithDocumentsAsync(int id)
    {
        return await _context.DocumentCategories
            .Include(c => c.Documents)
                .ThenInclude(d => d.User)
            .Include(c => c.Documents)
                .ThenInclude(d => d.DocumentType)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
} 