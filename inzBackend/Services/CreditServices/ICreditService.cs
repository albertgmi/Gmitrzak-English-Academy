using inzBackend.Models.CreditModels;

namespace inzBackend.Services.CreditServices
{
    public interface ICreditService
    {
        void AwardCredits(int userId, int amount, string reason);
        void CheckAndAwardDailyChallenge(int userId);
        void CheckAndAwardWeeklyChallenge(int userId);
        CreditSummaryDto GetCreditSummary(int userId);
        List<ShopItemDto> GetShopItems();
        List<ShopItemDto> GetShopItems(int userId);
        ShopPurchaseResultDto PurchaseItem(int userId, int shopItemId);
        List<UserCreditSummaryDto> GetAllUsersCreditSummary();
        StudentCreditDetailDto GetStudentCreditDetail(int studentId);
        void UpdatePurchaseStatus(int purchaseId, string status);
    }
}
