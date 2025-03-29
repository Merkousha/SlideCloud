using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Models;
using SlideCloud.Models.ContactUs;

namespace SlideCloud.Data;


public class AppDbContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentCategory> DocumentCategories { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }


    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagDocument> TagDocuments { get; set; }

    // add Contact model to database
    public DbSet<Contact> Contacts { get; set; }
}

