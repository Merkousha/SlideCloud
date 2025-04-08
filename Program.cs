using System.IO.Compression;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Areas;
using SlideCloud.Data;
using SlideCloud.Models;
using SlideCloud.Services;
using WebMarkupMin.AspNetCoreLatest;

namespace SlideCloud
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Env.Load();
            builder.Services.AddResponseCaching();
            // تنظیمات سطح فشرده‌سازی
            builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            builder.Services.AddScoped<IS3Uploader, S3Uploader>();

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>(); // فقط GZip
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "text/html",
                    "text/css",
                    "application/javascript"
                });
            });
     
            builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            #region AddIdentity and configure

            builder.Services.AddIdentity<User, IdentityRole<long>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDAxNEAzMjM4MkUzMTJFMzlMeXJkaVJFV2Z5R3o5ZXNEVnNOQjFqUmx2MW0xZkR2TGdud2MrVGNJRlBzPQ==");

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // تنظیمات اعتبارسنجی رمز عبور
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;

                // تنظیمات قفل کردن حساب کاربری
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // تنظیمات کوکی‌ها و احراز هویت
                options.SignIn.RequireConfirmedEmail = false;
            });
            builder.Services.AddScoped<RoleManager<IdentityRole<long>>>();
            #endregion

            // Add services to the container.
            var ConnectionString = Environment.GetEnvironmentVariable("DefaultConnection");

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(ConnectionString));
            builder.Services.AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                })
                .AddHtmlMinification()
                
                .AddHttpCompression()
                .AddXmlMinification();
            var app = builder.Build();
     
 
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseWebMarkupMin();
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
                    OnPrepareResponse = ctx =>
                    {

                        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=3600");
                    }
                });


            app.UseRouting();

            app.UseAuthentication(); // قبل از UseAuthorization قرار دارد
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


     

            app.MapRazorPages();
   
            // app.UseResponseCompression();
            app.Run();
        }
    }
}
