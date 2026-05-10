using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class ProgramCourseConfiguration : IEntityTypeConfiguration<ProgramCourse>
    {
        public void Configure(EntityTypeBuilder<ProgramCourse> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Program>().WithMany(p => p.ProgramCourses).HasForeignKey(x => x.ProgramId);
            builder.HasOne<Course>().WithMany().HasForeignKey(x => x.CourseId);
        }
    }
}
