using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class CatalogueConfiguration : IEntityTypeConfiguration<Catalogue>
    {
        public void Configure(EntityTypeBuilder<Catalogue> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedByUserId);
        }
    }
}
