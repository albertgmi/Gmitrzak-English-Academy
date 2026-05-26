using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class RankingReactionConfiguration : IEntityTypeConfiguration<RankingReaction>
    {
        public void Configure(EntityTypeBuilder<RankingReaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.FromUser).WithMany()
                .HasForeignKey(x => x.FromUserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.ToUser).WithMany()
                .HasForeignKey(x => x.ToUserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
