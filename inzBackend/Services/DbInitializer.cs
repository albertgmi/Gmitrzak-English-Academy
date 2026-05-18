using inzBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace inzBackend.Services
{
    public static class DbInitializer
    {
        public static void Seed(GmitrzakEnglishAcademyDbContext context)
        {
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<AppUser>();
                var now = DateTimeOffset.UtcNow;

                var admin = new AppUser
                {
                    Id = 99,
                    Username = "testadmin",
                    Email = "admin@example.com",
                    PasswordHash = hasher.HashPassword(null, "Barbara1"),
                    Role = Enums.UserRole.Admin,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreatedBy = "System",
                    LastModifiedAt = now,
                    LastModifiedBy = "System"
                };

                var user = new AppUser
                {
                    Id = 100,
                    Username = "testuser",
                    Email = "user@example.com",
                    PasswordHash = hasher.HashPassword(null, "Barbara1"),
                    Role = Enums.UserRole.User,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreatedBy = "System",
                    LastModifiedAt = now,
                    LastModifiedBy = "System"
                };

                context.Users.AddRange(admin, user);
                context.SaveChanges();
            }
        }
    }
}
