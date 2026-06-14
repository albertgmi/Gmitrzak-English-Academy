using inzBackend.Models.RankingModels;
using inzBackend.Services.RankingServices;
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

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        [HttpGet("{period}")]
        public ActionResult<RankingDto> getRanking(string period)
        {
            return _rankingService.getRanking(period);
        }

        [HttpPost("reaction")]
        public ActionResult addReaction([FromBody] AddReactionRequest request)
        {
            _rankingService.addReaction(request);
            return Ok();
        }

        [HttpDelete("reaction")]
        public ActionResult removeReaction([FromQuery] int toUserId, [FromQuery] string emoji, [FromQuery] string period)
        {
            _rankingService.removeReaction(toUserId, emoji, period);
            return Ok();
        }
    }
}
