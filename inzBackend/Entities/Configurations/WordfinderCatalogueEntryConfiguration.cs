using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using inzBackend.Entities.Resources;

namespace inzBackend.Entities.Configurations
{
    public class WordfinderCatalogueEntryConfiguration : IEntityTypeConfiguration<WordfinderCatalogueEntry>
    {
        public void Configure(EntityTypeBuilder<WordfinderCatalogueEntry> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.WordfinderCatalogue)
                .WithMany(x => x.Entries)
                .HasForeignKey(x => x.WordfinderCatalogueId);
        }
    }
}
