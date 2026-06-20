using inzBackend.Entities.Gamification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inzBackend.Entities.Configurations
{
    public class UserStreakShieldConfiguration : IEntityTypeConfiguration<UserStreakShield>
    {
        public void Configure(EntityTypeBuilder<UserStreakShield> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
