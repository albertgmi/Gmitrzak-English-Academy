using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class UserSentenceAssignmentConfiguration : IEntityTypeConfiguration<UserSentenceAssignment>
    {
        public void Configure(EntityTypeBuilder<UserSentenceAssignment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.SentenceSet).WithMany(s => s.Assignments)
                .HasForeignKey(x => x.SentenceSetId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            builder.HasOne(x => x.SentenceStock).WithMany()
                .HasForeignKey(x => x.SentenceStockId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
