using inzBackend.Models.CreditModels;

namespace inzBackend.Services.ShopActionServices
{
    public interface IShopActionService
    {
        ShopPurchaseResultDto SkipHomework(int userId, int moduleId);
    }
}