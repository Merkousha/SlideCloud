using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, Microsoft.AspNetCore.Identity.IdentityRole<long>, long>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<DocumentCategory> Categories { get; set; } = null!;
        public DbSet<DocumentType> DocumentTypes { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<TagDocument> TagDocuments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Document entity
            builder.Entity<Document>()
                .HasOne(d => d.DocumentType)
                .WithMany(dt => dt.Documents)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Document>()
                .HasOne(d => d.DocumentCategory)
                .WithMany(dc => dc.Documents)
                .HasForeignKey(d => d.DocumentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure TagDocument entity (many-to-many relationship)
            builder.Entity<TagDocument>()
                .HasKey(td => td.Id);

            builder.Entity<TagDocument>()
                .HasOne(td => td.Tag)
                .WithMany(t => t.TagDocuments)
                .HasForeignKey(td => td.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TagDocument>()
                .HasOne(td => td.Document)
                .WithMany(d => d.TagDocuments)
                .HasForeignKey(td => td.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 