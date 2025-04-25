using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Models.ContactUs;
using SlideCloud.Domain.Models.Blog;

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
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<Blog> Blogs { get; set; } = null!;

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

            // Configure Blog entity
            builder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Blog>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Blog>()
                .Property(b => b.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Blog>()
                .Property(b => b.Summary)
                .HasMaxLength(500);

            builder.Entity<Blog>()
                .Property(b => b.FeaturedImage)
                .HasMaxLength(500);

            builder.Entity<Blog>()
                .HasIndex(b => b.Slug)
                .IsUnique();
        }
    }
} 