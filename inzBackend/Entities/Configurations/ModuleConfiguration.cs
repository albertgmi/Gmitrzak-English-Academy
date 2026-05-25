using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.HasOne(m => m.TheaterItem)
                .WithMany()
                .HasForeignKey(m => m.TheaterItemId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
