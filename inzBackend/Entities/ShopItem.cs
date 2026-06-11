using inzBackend.Models;

namespace inzBackend.Entities
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
