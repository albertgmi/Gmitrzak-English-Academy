using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class CatalogueEntryConfiguration : IEntityTypeConfiguration<CatalogueEntry>
    {
        public void Configure(EntityTypeBuilder<CatalogueEntry> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Catalogue>().WithMany(c => c.Entries).HasForeignKey(x => x.CatalogueId);
        }
    }
}
