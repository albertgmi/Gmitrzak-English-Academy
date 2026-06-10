using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class ShopPurchaseConfiguration : IEntityTypeConfiguration<ShopPurchase>
    {
        public void Configure(EntityTypeBuilder<ShopPurchase> builder)
        {
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.HasOne(sp => sp.User)
                .WithMany()
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sp => sp.ShopItem)
                .WithMany()
                .HasForeignKey(sp => sp.ShopItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
