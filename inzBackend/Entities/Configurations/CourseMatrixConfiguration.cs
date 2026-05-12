using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class CourseMatrixConfiguration : IEntityTypeConfiguration<CourseMatrix>
    {
        public void Configure(EntityTypeBuilder<CourseMatrix> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne<Course>()
                .WithMany(c => c.CourseMatrices)
                .HasForeignKey(x => x.CourseId);

            builder.HasOne(cm => cm.Matrix)
                .WithMany()
                .HasForeignKey(x => x.MatrixId);
        }
    }
}
