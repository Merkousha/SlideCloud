using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.Interfaces;
using SlideCloud.Application.Services;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Data;
using SlideCloud.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
Env.Load();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
var ConnectionString = Environment.GetEnvironmentVariable("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(ConnectionString));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole<long>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();

// Add AWS S3
builder.Services.AddSingleton<IS3Uploader>(sp =>
{
    var bucketName = Environment.GetEnvironmentVariable("S3-BucketName") ?? "slidecloud-bucket";
    return new S3Uploader();
});

// Add UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Repositories
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();

// Add Services
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<IAdminSlideService, AdminSlideService>();
builder.Services.AddScoped<ISlideService, SlideService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IContactService, ContactService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();