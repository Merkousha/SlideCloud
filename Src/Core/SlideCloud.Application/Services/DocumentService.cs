using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DocumentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
    {
        return await _unitOfWork.Documents.GetAll()
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .ToListAsync();
    }

    public async Task<Document> GetDocumentByIdAsync(int id)
    {
        return await _unitOfWork.Documents.GetAll()
            .Include(d => d.DocumentCategory)
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetUserDocumentsAsync(string userId)
    {
        return await _unitOfWork.Documents.GetDocumentsByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm)
    {
        return await _unitOfWork.Documents.SearchDocumentsAsync(searchTerm);
    }

    public async Task<Document> CreateDocumentAsync(Document document)
    {
        await _unitOfWork.Documents.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();
        return document;
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        _unitOfWork.Documents.Update(document);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(int id)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(id);
        if (document != null)
        {
            _unitOfWork.Documents.Delete(document);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task IncrementViewCountAsync(int documentId)
    {
        await _unitOfWork.Documents.IncrementViewCountAsync(documentId);
    }
} 