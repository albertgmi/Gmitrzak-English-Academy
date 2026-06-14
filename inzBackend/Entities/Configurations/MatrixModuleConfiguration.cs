using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Curriculum;

namespace inzBackend.Models.Configurations
{
    public class MatrixModuleConfiguration : IEntityTypeConfiguration<MatrixModule>
    {
        public void Configure(EntityTypeBuilder<MatrixModule> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Matrix)
                .WithMany(m => m.MatrixModules)
                .HasForeignKey(x => x.MatrixId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Module)
                .WithMany(m => m.MatrixModules)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
