using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Curriculum;

namespace inzBackend.Models.Configurations
{
    public class ProgramCourseConfiguration : IEntityTypeConfiguration<ProgramCourse>
    {
        public void Configure(EntityTypeBuilder<ProgramCourse> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(pc => pc.Program)
                .WithMany(p => p.ProgramCourses)
                .HasForeignKey(pc => pc.ProgramId);

            builder.HasOne(pc => pc.Course)
                .WithMany(c=>c.ProgramCourses)
                .HasForeignKey(pc => pc.CourseId);
        }
    }
}
