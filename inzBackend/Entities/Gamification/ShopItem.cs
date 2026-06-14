using inzBackend.Entities.Base;

namespace inzBackend.Entities.Gamification
{
    public class ShopItem : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreditCost { get; set; }
        public bool IsActive { get; set; } = true;
        public string? IconEmoji { get; set; }
    }
}
