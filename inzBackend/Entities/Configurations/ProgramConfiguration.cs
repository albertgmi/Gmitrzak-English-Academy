using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Models.Configurations
{
    public class ProgramConfiguration : IEntityTypeConfiguration<Entities.Curriculum.Program>
    {
        public void Configure(EntityTypeBuilder<Entities.Curriculum.Program> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
