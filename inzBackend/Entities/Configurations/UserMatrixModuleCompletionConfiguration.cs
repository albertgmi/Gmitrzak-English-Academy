using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inzBackend.Entities.Configurations
{
    public class UserMatrixModuleCompletionConfiguration : IEntityTypeConfiguration<UserMatrixModuleCompletion>
    {
        public void Configure(EntityTypeBuilder<UserMatrixModuleCompletion> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                .WithMany(u => u.UserMatrixModuleCompletions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.MatrixModule)
                .WithMany()
                .HasForeignKey(x => x.MatrixModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.MatrixModuleId }).IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
