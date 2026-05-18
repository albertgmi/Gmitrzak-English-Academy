using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class TheaterItemConfiguration : IEntityTypeConfiguration<TheaterItem>
    {
        public void Configure(EntityTypeBuilder<TheaterItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
            builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.ThumbnailUrl).HasMaxLength(1000);
            builder.Property(x => x.MediaType).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Level).HasMaxLength(30);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
