using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.Business.Models;
using myshop.DataAccess.Data;
using myshop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppDbContext context;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }
        public async Task Initialize()
        {
            // Migration
            try
            {
                if (context.Database.GetPendingMigrations().Count() > 0)
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Roles
            if (!await roleManager.RoleExistsAsync(SD.AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
                await roleManager.CreateAsync(new IdentityRole(SD.EditorRole));
                await roleManager.CreateAsync(new IdentityRole(SD.CustomerRole));



                // Users
                await userManager.CreateAsync(new ApplicationUser
                { 
                    UserName = "Admin@myshop.com",
                    Email = "Admin@myshop.com",
                    Name = "Administrator",
                    PhoneNumber = "1234567890",
                    Address = "123 Admin St",
                    City = "Admin City",
                },"P@$$w0rd");

                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "Admin@myshop.com");
                if (user != null)
                    await userManager.AddToRoleAsync(user, SD.AdminRole);
            }


            return;
        }
    }
}
