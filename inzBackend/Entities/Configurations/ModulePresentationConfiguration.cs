using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class ModulePresentationConfiguration : IEntityTypeConfiguration<ModulePresentation>
    {
        public void Configure(EntityTypeBuilder<ModulePresentation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Module)
                .WithOne(m => m.Presentation)
                .HasForeignKey<ModulePresentation>(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
