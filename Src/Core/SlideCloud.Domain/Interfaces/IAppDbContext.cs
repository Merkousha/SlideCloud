using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Domain.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Document> Documents { get; set; }
        DbSet<DocumentCategory> Categories { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 