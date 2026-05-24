using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using inzBackend.Helpers;

namespace inzBackend.Models.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
            builder.HasQueryFilter(x => !x.IsDeleted);
            var hasher = new PasswordHasher<AppUser>();

            builder.HasData(
                new AppUser
                {
                    Id = 99,
                    Username = "testadmin",
                    Email = "admin@example.com",
                    PasswordHash = hasher.HashPassword(null, "Barbara1"),
                    Role = Enums.UserRole.Admin,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = PolandTime.DateTimeNow,
                    CreatedBy = "System",
                    LastModifiedAt = PolandTime.DateTimeNow,
                    LastModifiedBy = "System"
                },
                new AppUser
                {
                    Id = 100,
                    Username = "testauser",
                    Email = "user@example.com",
                    PasswordHash = hasher.HashPassword(null, "Barbara1"),
                    Role = Enums.UserRole.User,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = PolandTime.DateTimeNow,
                    CreatedBy = "System",
                    LastModifiedAt = PolandTime.DateTimeNow,
                    LastModifiedBy = "System"
                }
            );
        }
    }
}
