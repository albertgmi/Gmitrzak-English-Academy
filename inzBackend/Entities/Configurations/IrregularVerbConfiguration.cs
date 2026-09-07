using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using inzBackend.Entities.Identity;
using inzBackend.Entities.LearningMaterials;

namespace inzBackend.Entities.Configurations
{
    public class IrregularVerbConfiguration : IEntityTypeConfiguration<IrregularVerb>
    {
        public void Configure(EntityTypeBuilder<IrregularVerb> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.PolishTranslation)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(x => x.EnglishForms)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(x => x.Level)
                   .IsRequired();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
