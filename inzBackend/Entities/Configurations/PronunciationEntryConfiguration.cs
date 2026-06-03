using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class PronunciationEntryConfiguration : IEntityTypeConfiguration<PronunciationEntry>
    {
        public void Configure(EntityTypeBuilder<PronunciationEntry> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
            builder.Property(x => x.Status).HasConversion<string>();
        }
    }
}
