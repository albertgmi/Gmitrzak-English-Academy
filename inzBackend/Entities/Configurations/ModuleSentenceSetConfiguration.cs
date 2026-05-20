using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class ModuleSentenceSetConfiguration : IEntityTypeConfiguration<ModuleSentenceSet>
    {
        public void Configure(EntityTypeBuilder<ModuleSentenceSet> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Module).WithMany()
                .HasForeignKey(x => x.ModuleId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.SentenceSet).WithMany()
                .HasForeignKey(x => x.SentenceSetId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
