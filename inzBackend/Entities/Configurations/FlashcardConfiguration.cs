using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Identity;
using inzBackend.Entities.SpacedRepetition;

namespace inzBackend.Models.Configurations
{
    public class FlashcardConfiguration : IEntityTypeConfiguration<Flashcard>
    {
        public void Configure(EntityTypeBuilder<Flashcard> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Vocabulary)
                   .WithMany(v => v.Flashcards)
                   .HasForeignKey(x => x.VocabularyId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}
