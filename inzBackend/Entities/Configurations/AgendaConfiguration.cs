using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.Administration;

namespace inzBackend.Entities.Configurations
{
    public class AgendaConfiguration : IEntityTypeConfiguration<Agenda>
    {
        public void Configure(EntityTypeBuilder<Agenda> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithOne()
                .HasForeignKey<Agenda>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
