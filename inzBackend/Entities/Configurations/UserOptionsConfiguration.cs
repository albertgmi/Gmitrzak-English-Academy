using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class UserOptionsConfiguration : IEntityTypeConfiguration<UserOptions>
    {
        public void Configure(EntityTypeBuilder<UserOptions> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithOne()
                .HasForeignKey<UserOptions>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
