using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using inzBackend.Entities.Resources;

namespace inzBackend.Entities.Configurations
{
    public class WordfinderCatalogueConfiguration : IEntityTypeConfiguration<WordfinderCatalogue>
    {
        public void Configure(EntityTypeBuilder<WordfinderCatalogue> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.StudentUser)
                .WithMany()
                .HasForeignKey(x => x.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ApprovedCatalogue)
                .WithMany()
                .HasForeignKey(x => x.ApprovedCatalogueId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Entries)
                .WithOne(x => x.WordfinderCatalogue)
                .HasForeignKey(x => x.WordfinderCatalogueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
