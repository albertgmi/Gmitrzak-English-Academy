using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Assignments;

namespace inzBackend.Models.Configurations
{
    public class ListeningReportConfiguration : IEntityTypeConfiguration<ListeningReport>
    {
        public void Configure(EntityTypeBuilder<ListeningReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
            builder.Property(x => x.MediaType).HasConversion<string>().HasMaxLength(50);
        }
    }
}
