using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Entities;

namespace SlideCloud.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<DocumentCategory> DocumentCategories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagDocument> TagDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasMany(u => u.Documents)
            .WithOne(d => d.User)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Document entity
        modelBuilder.Entity<Document>()
            .HasOne(d => d.DocumentType)
            .WithMany(dt => dt.Documents)
            .HasForeignKey(d => d.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.DocumentCategory)
            .WithMany(dc => dc.Documents)
            .HasForeignKey(d => d.DocumentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure TagDocument entity (many-to-many relationship)
        modelBuilder.Entity<TagDocument>()
            .HasKey(td => td.Id);

        modelBuilder.Entity<TagDocument>()
            .HasOne(td => td.Tag)
            .WithMany(t => t.TagDocuments)
            .HasForeignKey(td => td.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TagDocument>()
            .HasOne(td => td.Document)
            .WithMany(d => d.TagDocuments)
            .HasForeignKey(td => td.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 