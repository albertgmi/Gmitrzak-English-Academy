using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.LearningMaterials;

namespace inzBackend.Entities.Configurations
{
    public class SentenceSetConfiguration : IEntityTypeConfiguration<SentenceSet>
    {
        public void Configure(EntityTypeBuilder<SentenceSet> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.GroupName).HasMaxLength(200);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
