using inzBackend.Entities.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inzBackend.Entities.Configurations
{
    public class UserMatrixModuleDueDateOverrideConfiguration : IEntityTypeConfiguration<UserMatrixModuleDueDateOverride>
    {
        public void Configure(EntityTypeBuilder<UserMatrixModuleDueDateOverride> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.MatrixModule).WithMany().HasForeignKey(x => x.MatrixModuleId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => new { x.UserId, x.MatrixModuleId }).IsUnique();
        }
    }
}
