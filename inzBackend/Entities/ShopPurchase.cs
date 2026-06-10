using inzBackend.Models;

namespace inzBackend.Entities
{
    public class ShopPurchase : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int ShopItemId { get; set; }
        public ShopItem ShopItem { get; set; } = null!;
        public int CreditCost { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
