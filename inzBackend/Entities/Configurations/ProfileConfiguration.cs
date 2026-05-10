using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AvatarUrl).HasMaxLength(500);
            builder.Property(x => x.EnglishLevel).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.CurrentSemester).HasDefaultValue(1);

            builder.HasOne(x => x.User)
                   .WithOne()
                   .HasForeignKey<Profile>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
