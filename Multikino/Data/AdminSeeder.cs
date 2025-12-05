// Data/AdminSeeder.cs
using Microsoft.EntityFrameworkCore;
using Multikino.Models;
using Multikino.Services;

namespace Multikino.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(MultikinoDbContext context)
        {
            // jeśli już istnieje admin, nic nie rób
            if (await context.Users.AnyAsync(u => u.Role == "Admin"))
                return;

            const string adminUserName = "admin";
            const string adminPassword = "admin";

            PasswordHasher.CreatePasswordHash(adminPassword, out var hash, out var salt);

            var admin = new User
            {
                UserName = adminUserName,
                Email = "admin@local",
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
