using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Assignments;

namespace inzBackend.Entities.Configurations
{
    public class UserSentenceAnswerConfiguration : IEntityTypeConfiguration<UserSentenceAnswer>
    {
        public void Configure(EntityTypeBuilder<UserSentenceAnswer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Module).WithMany()
                .HasForeignKey(x => x.ModuleId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.SentenceStock).WithMany()
                .HasForeignKey(x => x.SentenceStockId).OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
