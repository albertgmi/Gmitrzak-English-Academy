using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class UserMatrixAssignmentConfiguration : IEntityTypeConfiguration<UserMatrixAssignment>
    {
        public void Configure(EntityTypeBuilder<UserMatrixAssignment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId);
            builder.HasOne<Matrix>().WithMany().HasForeignKey(x => x.MatrixId);
        }
    }
}
