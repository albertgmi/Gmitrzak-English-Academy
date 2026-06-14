using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Curriculum;

namespace inzBackend.Models.Configurations
{
    public class MatrixConfiguration : IEntityTypeConfiguration<Matrix>
    {
        public void Configure(EntityTypeBuilder<Matrix> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
