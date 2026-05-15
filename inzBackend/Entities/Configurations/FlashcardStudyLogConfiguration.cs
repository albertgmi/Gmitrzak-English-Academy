using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class FlashcardStudyLogConfiguration : IEntityTypeConfiguration<FlashcardStudyLog>
    {
        public void Configure(EntityTypeBuilder<FlashcardStudyLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Flashcard)
                .WithMany()
                .HasForeignKey(x => x.FlashcardId);

            builder.HasOne(x => x.User)
                .WithMany(u => u.FlashcardStudyLogs)
                .HasForeignKey(x => x.UserId);
        }
    }
}
