using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class SentenceStockConfiguration : IEntityTypeConfiguration<SentenceStock>
    {
        public void Configure(EntityTypeBuilder<SentenceStock> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Polish).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.EnglishTranslation).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.Category).HasMaxLength(100);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
