namespace SlideCloud.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IDocumentRepository Documents { get; }
        ICategoryRepository Categories { get; }
        IDocumentTypeRepository DocumentTypes { get; }
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync();
    }
}