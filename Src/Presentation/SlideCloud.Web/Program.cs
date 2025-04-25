using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.Interfaces;
using SlideCloud.Application.Services;
using SlideCloud.Domain.Entities;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Data;
using SlideCloud.Infrastructure.Repositories;
using Sentry;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://bc38a43cefded4a3a06d505a00c9c207@o4506168508940288.ingest.us.sentry.io/4509213508370432";
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
});

Env.Load();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
var ConnectionString = "Data Source=sahand.liara.cloud,34477;Initial Catalog=slidecloud;User Id=sa;Password=eqezZhqlE4FCzR5j9EykdooT;Encrypt=False;";
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
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


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
builder.Services.AddScoped<IPresentationGeneratorService, PresentationGeneratorService>();
builder.Services.AddScoped<IBlogService, BlogService>();


var app = builder.Build();
app.UseSession();
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
    name: "slideDetail",
    pattern: "Slide/Detail/{id:int}/{slug?}",
    defaults: new { controller = "Slide", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();