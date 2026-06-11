namespace inzBackend.Models.CreditModels
{
    public class CreditSummaryDto
    {
        public int TotalCredits { get; set; }
        public int CreditsEarned { get; set; }
        public int CreditsSpent { get; set; }
        public List<CreditHistoryItemDto> History { get; set; } = [];
        public List<ShopPurchaseDto> Purchases { get; set; } = [];
    }
}
