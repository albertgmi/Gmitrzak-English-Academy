namespace inzBackend.Models.CreditModels
{
    public class ShopItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreditCost { get; set; }
        public string? IconEmoji { get; set; }
        public bool CanAfford { get; set; }
    }
}
