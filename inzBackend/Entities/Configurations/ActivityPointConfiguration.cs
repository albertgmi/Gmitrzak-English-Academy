using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class ActivityPointConfiguration : IEntityTypeConfiguration<ActivityPoint>
    {
        public void Configure(EntityTypeBuilder<ActivityPoint> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
