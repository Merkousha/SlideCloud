using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Areas;
using SlideCloud.Data;
using SlideCloud.Models;
using SlideCloud.Services;

namespace SlideCloud
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Env.Load();

            builder.Services.AddScoped<IS3Uploader, S3Uploader>();


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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // قبل از UseAuthorization قرار دارد
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

            app.MapRazorPages();

            app.Run();
        }
    }
}
