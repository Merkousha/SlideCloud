using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlideCloud.Data;
using SlideCloud.Models;

namespace SlideCloud
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region AddIdentity and configure

            builder.Services.AddIdentity<User, IdentityRole<long>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


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
            //builder.Services.AddScoped<RoleManager<ApplicationRole>>();
            #endregion

            // Add services to the container.
            #region Definee ConnectionString
#if DEBUG
            string ConnectionString = builder.Configuration.GetConnectionString("LocalConnection");
#else
            string ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
#endif
            #endregion

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

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
