using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class MatrixModuleConfiguration : IEntityTypeConfiguration<MatrixModule>
    {
        public void Configure(EntityTypeBuilder<MatrixModule> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Matrix>().WithMany(m => m.MatrixModules).HasForeignKey(x => x.MatrixId);
            builder.HasOne<Module>().WithMany().HasForeignKey(x => x.ModuleId);
        }
    }
}
