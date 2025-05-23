namespace SlideCloud.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IDocumentRepository Documents { get; }
        ICategoryRepository Categories { get; }
        IDocumentTypeRepository DocumentTypes { get; }
        IUserRepository Users { get; }
        IContactRepository Contacts { get; }
        IBlogRepository Blogs { get; }
        Task<int> SaveChangesAsync();
    }
}