using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class GlobalFlashcardConfiguration : IEntityTypeConfiguration<GlobalFlashcard>
    {
        public void Configure(EntityTypeBuilder<GlobalFlashcard> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Front).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Back).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Category).HasMaxLength(100);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
