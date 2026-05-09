using Backend.Backend.Model;
using Microsoft.AspNetCore.Identity;

namespace Backend.Backend.Data.Seeders
{
    /// <summary>
    /// Seeds the default administrator user into the database.
    /// This ensures the system always has at least one admin account on startup.
    /// </summary>
    public static class AdminUserSeeder
    {
        /// <summary>
        /// Creates the default admin user if it does not already exist.
        /// Also ensures the Admin role exists and assigns it to the user.
        /// </summary>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        /// <param name="roleManager">ASP.NET Core Identity role manager.</param>
        public static async Task SeedAdminAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@admin.local";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
                return;

            string passhash = BCrypt.Net.BCrypt.HashPassword("123");

            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,

                PasswordHash = passhash,
                DocumentSeries = "ADM-2026-1",
                Full_Name = "System Administrator",
                Phone_Number = null,
                Sex = null,
                Birth_Date = null,
                Address = "System Generated",

                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM",
                LastUpdatedBy = "SYSTEM",

                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@12345!");

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}