using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Curriculum;

namespace inzBackend.Models.Configurations
{
    public class CourseMatrixConfiguration : IEntityTypeConfiguration<CourseMatrix>
    {
        public void Configure(EntityTypeBuilder<CourseMatrix> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(cm => cm.Course)
                .WithMany(c => c.CourseMatrices)
                .HasForeignKey(cm => cm.CourseId);

            builder.HasOne(cm => cm.Matrix)
                .WithMany(m => m.CourseMatrices)
                .HasForeignKey(cm => cm.MatrixId);
        }
    }
}
