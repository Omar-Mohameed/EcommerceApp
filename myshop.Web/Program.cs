using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.DataAccess.Data;
using myshop.DataAccess.Implementation;
using myshop.Utilities;
using Stripe;

namespace myshop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
            ));
            // stripe settings
            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("Stripe"));

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            // Add Identity
            builder.Services.AddIdentity<IdentityUser,IdentityRole>(
                options=>options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromHours(1))
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

            // add httpcontext accessor service
            builder.Services.AddHttpContextAccessor();

            // Session Service
            builder.Services.AddDistributedMemoryCache(); // In-memory cache for session
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            // Session Middleware
            app.UseSession();

            app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                //pattern: "{area=Admin}/{controller=Product}/{action=Index}/{id?}");
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
