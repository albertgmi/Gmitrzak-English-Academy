using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Administration;

namespace inzBackend.Entities.Configurations
{
    public class AnnouncementRecipientConfiguration : IEntityTypeConfiguration<AnnouncementRecipient>
    {
        public void Configure(EntityTypeBuilder<AnnouncementRecipient> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Announcement).WithMany(a => a.Recipients)
                .HasForeignKey(x => x.AnnouncementId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
