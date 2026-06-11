namespace inzBackend.Models.CreditModels
{
    public class StudentCreditDetailDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int TotalCredits { get; set; }
        public int CreditsEarned { get; set; }
        public int CreditsSpent { get; set; }
        public List<CreditHistoryItemDto> History { get; set; }
        public List<ShopPurchaseDto> Purchases { get; set; }
    }
}
