
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Models.Database;

namespace MovieReservationSystem.Helpers
{
    public class AppDbInitilizer
    {
        public static async Task InitializeAsync(
        appDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
        // 1. Ensure migrations are applied
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            // 2. Seed Roles
            string[] roles = { "Admin", "User"};
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 3. Seed Default Admin User
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    AccountType = Enums.AccountType.Admin
                };

                // Create user with a secure password (hashed automatically)
                var createResult = await userManager.CreateAsync(adminUser, "Admin@123456");

                if (createResult.Succeeded)
                {
                    // Assign role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}