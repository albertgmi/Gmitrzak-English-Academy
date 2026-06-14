using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Administration;

namespace inzBackend.Entities.Configurations
{
    public class TeacherNoteConfiguration : IEntityTypeConfiguration<TeacherNote>
    {
        public void Configure(EntityTypeBuilder<TeacherNote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).IsRequired();
            builder.HasOne(x => x.Teacher).WithMany()
                .HasForeignKey(x => x.TeacherUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Student).WithMany()
                .HasForeignKey(x => x.StudentUserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
