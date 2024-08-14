using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Commons.Utilities
{
    public static class Seeder
    {
        public static async Task SeedRolesAndSuperAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Seed roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole("Admin");
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                var role = new IdentityRole("User");
                await roleManager.CreateAsync(role);
            }

            // Seed super admin user
            if (userManager.FindByNameAsync("Admin").Result == null)
            {
                var user = new AppUser
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                var result = userManager.CreateAsync(user, "Password@123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        } 

    }
}
