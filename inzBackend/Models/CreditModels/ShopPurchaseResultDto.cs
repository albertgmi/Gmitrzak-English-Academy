namespace inzBackend.Models.CreditModels
{
    public class ShopPurchaseResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CreditsRemaining { get; set; }
        public int? PurchaseId { get; set; }
    }
}
