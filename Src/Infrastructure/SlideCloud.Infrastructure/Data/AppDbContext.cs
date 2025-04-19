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

            // Add any additional model configurations here
        }
    }
} 