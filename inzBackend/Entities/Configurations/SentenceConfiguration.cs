using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Identity;
using inzBackend.Entities.LearningMaterials;

namespace inzBackend.Models.Configurations
{
    public class SentenceConfiguration : IEntityTypeConfiguration<Sentence>
    {
        public void Configure(EntityTypeBuilder<Sentence> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);
            builder.Property(x => x.Content).IsRequired();
        }
    }
}
