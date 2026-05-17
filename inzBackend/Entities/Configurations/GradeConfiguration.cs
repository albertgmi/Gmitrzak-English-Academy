using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
            builder.Property(x => x.Percentage).HasPrecision(5, 2);
        }
    }
}
