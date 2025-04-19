using SlideCloud.Domain.Entities;

namespace SlideCloud.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<DocumentCategory>
    {
        Task<IEnumerable<DocumentCategory>> GetActiveCategoriesAsync();
        Task<DocumentCategory> GetCategoryWithDocumentsAsync(int id);
    }
}