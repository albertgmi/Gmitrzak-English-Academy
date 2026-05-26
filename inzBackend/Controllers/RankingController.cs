using inzBackend.Models.RankingModels;
using inzBackend.Services.RankingServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/ranking")]
    [ApiController]
    [Authorize]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;
        private readonly IUserContextService _userContextService;

        public RankingController(IRankingService rankingService, IUserContextService userContextService)
        {
            _rankingService = rankingService;
            _userContextService = userContextService;
        }

        [HttpGet("{period}")]
        public RankingDto getRanking(string period)
        {
            return _rankingService.getRanking(period);
        }

        [HttpPost("reaction")]
        public ActionResult addReaction([FromBody] AddReactionRequest request)
        {
            var userId = _userContextService.GetUserId!.Value;
            _rankingService.addReaction(userId, request);
            return Ok();
        }

        [HttpDelete("reaction")]
        public ActionResult removeReaction([FromQuery] int toUserId, [FromQuery] string emoji, [FromQuery] string period)
        {
            var userId = _userContextService.GetUserId!.Value;
            _rankingService.removeReaction(userId, toUserId, emoji, period);
            return Ok();
        }
    }
}
