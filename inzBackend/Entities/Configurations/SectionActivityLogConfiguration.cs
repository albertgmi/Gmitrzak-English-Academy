using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Assignments;

namespace inzBackend.Entities.Configurations
{
    public class SectionActivityLogConfiguration : IEntityTypeConfiguration<SectionActivityLog>
    {
        public void Configure(EntityTypeBuilder<SectionActivityLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => new { x.UserId, x.Section, x.ActivityDate }).IsUnique();
        }
    }
}
