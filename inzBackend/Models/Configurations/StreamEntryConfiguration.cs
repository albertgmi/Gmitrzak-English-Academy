using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class StreamEntryConfiguration : IEntityTypeConfiguration<StreamEntry>
    {
        public void Configure(EntityTypeBuilder<StreamEntry> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);
        }
    }
}
