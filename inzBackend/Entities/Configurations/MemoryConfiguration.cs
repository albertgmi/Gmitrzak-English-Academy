using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Identity;
using inzBackend.Entities.SpacedRepetition;

namespace inzBackend.Models.Configurations
{
    public class MemoryConfiguration : IEntityTypeConfiguration<Memory>
    {
        public void Configure(EntityTypeBuilder<Memory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
