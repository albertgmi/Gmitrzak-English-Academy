using inzBackend.Models.CreditModels;
using inzBackend.Services.CreditServices;
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

        public CreditsController(
            ICreditService creditService,
            IUserContextService userContextService)
        {
            _creditService = creditService;
            _userContextService = userContextService;
        }

        [HttpGet("summary")]
        public ActionResult<CreditSummaryDto> getSummary()
        {
            var userId = _userContextService.GetUserId!.Value;
            return Ok(_creditService.getCreditSummary(userId));
        }

        [HttpGet("shop")]
        public ActionResult<List<ShopItemDto>> getShop()
        {
            var userId = _userContextService.GetUserId!.Value;
            return Ok(_creditService.getShopItems(userId));
        }

        [HttpPost("shop/purchase/{itemId}")]
        public ActionResult<ShopPurchaseResultDto> purchase(int itemId)
        {
            var userId = _userContextService.GetUserId!.Value;
            var result = _creditService.purchaseItem(userId, itemId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
