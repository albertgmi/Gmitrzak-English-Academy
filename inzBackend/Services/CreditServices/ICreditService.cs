using inzBackend.Models.CreditModels;

namespace inzBackend.Services.CreditServices
{
    public interface ICreditService
    {
        void awardCredits(int userId, int amount, string reason);
        void checkAndAwardDailyChallenge(int userId);
        void checkAndAwardWeeklyChallenge(int userId);
        CreditSummaryDto getCreditSummary(int userId);
        List<ShopItemDto> getShopItems();
        List<ShopItemDto> getShopItems(int userId);
        ShopPurchaseResultDto purchaseItem(int userId, int shopItemId);
    }
}
