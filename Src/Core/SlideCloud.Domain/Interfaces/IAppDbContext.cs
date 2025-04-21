using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.ContactUs;

namespace SlideCloud.Domain.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Document> Documents { get; set; }
        DbSet<DocumentCategory> Categories { get; set; }
        DbSet<DocumentType> DocumentTypes { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<TagDocument> TagDocuments { get; set; }
        DbSet<Contact> Contacts { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 