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
        public ActionResult<RankingDto> GetRanking(string period)
        {
            return _rankingService.GetRanking(period);
        }

        [HttpPost("reaction")]
        public ActionResult AddReaction([FromBody] AddReactionRequest request)
        {
            _rankingService.AddReaction(request);
            return Ok();
        }

        [HttpDelete("reaction")]
        public ActionResult RemoveReaction([FromQuery] int toUserId, [FromQuery] string emoji, [FromQuery] string period)
        {
            _rankingService.RemoveReaction(toUserId, emoji, period);
            return Ok();
        }
    }
}
