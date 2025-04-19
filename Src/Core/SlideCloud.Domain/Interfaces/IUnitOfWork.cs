using SlideCloud.Domain.Interfaces;

namespace SlideCloud.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDocumentRepository Documents { get; }
        ICategoryRepository Categories { get; }
        IDocumentTypeRepository DocumentTypes { get; }
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync();
    }
} 