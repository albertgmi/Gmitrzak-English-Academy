using DocumentFormat.OpenXml.Vml.Office;
using inzBackend.Entities.LearningMaterials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inzBackend.Entities.Configurations
{
    public class PronunciationAttemptConfiguration : IEntityTypeConfiguration<PronunciationAttempt>
    {
        public void Configure(EntityTypeBuilder<PronunciationAttempt> builder)
        {
            builder.HasOne(pa => pa.PronunciationEntry)
                .WithMany(pe => pe.Attempts)
                .HasForeignKey(pa => pa.PronunciationEntryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
