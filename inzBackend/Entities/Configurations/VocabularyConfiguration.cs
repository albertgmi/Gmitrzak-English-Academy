using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.LearningMaterials;

namespace inzBackend.Entities.Configurations
{
    public class VocabularyConfiguration : IEntityTypeConfiguration<Vocabulary>
    {
        public void Configure(EntityTypeBuilder<Vocabulary> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Front).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Back).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Category).HasMaxLength(100);

            builder.HasOne(x => x.Catalogue)
                   .WithMany()
                   .HasForeignKey(x => x.CatalogueId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
