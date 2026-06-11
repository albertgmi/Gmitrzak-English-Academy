namespace inzBackend.Models.CreditModels
{
    public class ShopPurchaseDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? IconEmoji { get; set; }
        public int CreditCost { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
