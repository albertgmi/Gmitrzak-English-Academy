using inzBackend.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inzBackend.Entities.Configurations
{
    public class UserMenuVisibilityConfiguration : IEntityTypeConfiguration<UserMenuVisibility>
    {
        public void Configure(EntityTypeBuilder<UserMenuVisibility> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MenuItemKey).IsRequired().HasMaxLength(150);
            builder.HasIndex(x => new { x.UserId, x.MenuItemKey }).IsUnique();
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
