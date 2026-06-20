using inzBackend.Models.CreditModels;

namespace inzBackend.Services.ShopActionServices
{
    public interface IShopActionService
    {
        ShopPurchaseResultDto SkipHomework(int moduleId);
        ShopPurchaseResultDto ExtendHomework(int assignmentId, DateOnly newDueDate);
        ShopPurchaseResultDto PurchasePointsBoost();
        ShopPurchaseResultDto PurchaseStreakShield();
    }
}