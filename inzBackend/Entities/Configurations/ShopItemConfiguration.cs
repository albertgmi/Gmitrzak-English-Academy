using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Entities.Configurations
{
    public class ShopItemConfiguration : IEntityTypeConfiguration<ShopItem>
    {
        public void Configure(EntityTypeBuilder<ShopItem> builder)
        {
            builder.HasKey(si => si.Id);

            builder.Property(si => si.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(si => si.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(si => si.IconEmoji)
                .HasMaxLength(10);

            builder.HasData(
                new ShopItem { Id = 1, Name = "Skip homework pass", Description = "Lorem ipsum dolor sit amet, skip one assignment without penalty.", CreditCost = 10, IsActive = true, IconEmoji = "📝" },
                new ShopItem { Id = 2, Name = "Bonus lesson", Description = "Lorem ipsum consectetur, get an extra 30-minute conversation session.", CreditCost = 20, IsActive = true, IconEmoji = "🎓" },
                new ShopItem { Id = 3, Name = "Custom flashcard pack", Description = "Lorem ipsum adipiscing, request a custom flashcard set on any topic.", CreditCost = 15, IsActive = true, IconEmoji = "🃏" },
                new ShopItem { Id = 4, Name = "Grammar deep-dive", Description = "Lorem ipsum elit, dedicated 45-minute grammar troubleshooting session.", CreditCost = 25, IsActive = true, IconEmoji = "📚" },
                new ShopItem { Id = 5, Name = "Pronunciation audit", Description = "Lorem ipsum sed, full pronunciation review with personalized feedback.", CreditCost = 30, IsActive = true, IconEmoji = "🎤" }
            );
        }
    }
}
