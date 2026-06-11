namespace inzBackend.Models.CreditModels
{
    public class UserCreditSummaryDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int TotalCredits { get; set; }
        public int CreditsEarned { get; set; }
        public int CreditsSpent { get; set; }
        public int PurchaseCount { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
