using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class UserMatrixAssignmentConfiguration : IEntityTypeConfiguration<UserMatrixAssignment>
    {
        public void Configure(EntityTypeBuilder<UserMatrixAssignment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Matrix)
                .WithMany()
                .HasForeignKey(x => x.MatrixId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
