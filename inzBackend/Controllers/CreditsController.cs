using inzBackend.Models.CreditModels;
using inzBackend.Services.CreditServices;
using inzBackend.Services.ShopActionServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/credits")]
    [ApiController]
    [Authorize]
    public class CreditsController : ControllerBase
    {
        private readonly ICreditService _creditService;
        private readonly IUserContextService _userContextService;
        private readonly IShopActionService _shopActionService;

        public CreditsController(ICreditService creditService, IUserContextService userContextService, IShopActionService shopActionService)
        {
            _creditService = creditService;
            _userContextService = userContextService;
            _shopActionService = shopActionService;
        }

        [HttpGet("summary")]
        public ActionResult<CreditSummaryDto> GetSummary()
        {
            var userId = _userContextService.GetUserId!.Value;
            return Ok(_creditService.GetCreditSummary(userId));
        }

        [HttpGet("shop")]
        public ActionResult<List<ShopItemDto>> GetShop()
        {
            var userId = _userContextService.GetUserId!.Value;
            return Ok(_creditService.GetShopItems(userId));
        }

        [HttpPost("shop/purchase/{itemId}")]
        public ActionResult<ShopPurchaseResultDto> Purchase(int itemId)
        {
            var userId = _userContextService.GetUserId!.Value;
            var result = _creditService.PurchaseItem(userId, itemId);
            return Ok(result);
        }

        [HttpPost("shop/action/skip-homework")]
        public ActionResult<ShopPurchaseResultDto> PurchaseHomeworkSkip([FromBody] SkipHomeworkRequestDto request)
        {
            var result = _shopActionService.SkipHomework(request.AssignmentId);
            return Ok(result);
        }

        [HttpPost("shop/action/extend-homework")]
        public ActionResult<ShopPurchaseResultDto> ExtendHomework([FromBody] ExtendHomeworkRequestDto request)
        {
            var dateOnly = DateOnly.FromDateTime(request.NewDueDate);
            var result = _shopActionService.ExtendHomework(request.AssignmentId, dateOnly);
            return Ok(result);
        }

        [HttpPost("shop/action/points-boost")]
        public ActionResult<ShopPurchaseResultDto> PurchasePointsBoost()
        {
            var result = _shopActionService.PurchasePointsBoost();
            return Ok(result);
        }

        [HttpPost("shop/action/streak-shield")]
        public ActionResult<ShopPurchaseResultDto> PurchaseStreakShield()
        {
            var result = _shopActionService.PurchaseStreakShield();
            return Ok(result);
        }
    }
}
