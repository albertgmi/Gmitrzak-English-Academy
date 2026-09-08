using inzBackend.Enums;
using inzBackend.Models.AiSpellCheckingModels;
using inzBackend.Models.WordfinderModels;
using inzBackend.Services.UserServices;
using inzBackend.Services.WordfinderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/wordfinder")]
    [ApiController]
    [Authorize]
    public class WordfinderController : ControllerBase
    {
        private readonly IWordfinderService _wordfinderService;
        private readonly IUserContextService _userContextService;

        public WordfinderController(
            IWordfinderService wordfinderService,
            IUserContextService userContextService)
        {
            _wordfinderService = wordfinderService;
            _userContextService = userContextService;
        }

        [HttpPost]
        public async Task<ActionResult<WordfinderCatalogueDto>> CreateDraft([FromBody] CreateWordfinderCatalogueDto dto)
        {
            var userId = _userContextService.GetUserId!.Value;
            var result = await _wordfinderService.CreateDraftAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<WordfinderCatalogueListDto>>> GetMyCatalogues()
        {
            var userId = _userContextService.GetUserId!.Value;
            var result = await _wordfinderService.GetMyCataloguesAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WordfinderCatalogueDto>> GetById(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            var isAdmin = User.IsInRole("Admin");
            var result = await _wordfinderService.GetByIdAsync(id, userId, isAdmin);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WordfinderCatalogueDto>> UpdateDraft(int id, [FromBody] UpdateWordfinderCatalogueDto dto)
        {
            var userId = _userContextService.GetUserId!.Value;
            var result = await _wordfinderService.UpdateDraftAsync(id, userId, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDraft(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            await _wordfinderService.DeleteDraftAsync(id, userId);
            return NoContent();
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<WordfinderCatalogueDto>> Submit(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            var result = await _wordfinderService.SubmitAsync(id, userId);
            return Ok(result);
        }

        [HttpPost("translate")]
        public async Task<ActionResult<TranslateWordfinderEntryResponse>> TranslateEntry([FromBody] TranslateWordfinderEntryRequest request)
        {
            var result = await _wordfinderService.TranslateEntryAsync(request);
            return Ok(result);
        }

        [HttpPost("spellcheck")]
        public async Task<ActionResult<SpellCheckResult>> SpellCheckEntry([FromBody] SpellCheckRequest request)
        {
            var result = await _wordfinderService.SpellCheckEntryAsync(request);
            return Ok(result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<WordfinderCatalogueListDto>>> GetPendingCatalogues([FromQuery] WordfinderCatalogueStatus? status = null)
        {
            var result = await _wordfinderService.GetPendingCataloguesAsync(status);
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WordfinderCatalogueDto>> Approve(int id, [FromBody] ApproveWordfinderCatalogueRequest? request)
        {
            var adminUserId = _userContextService.GetUserId!.Value;
            var result = await _wordfinderService.ApproveCatalogueAsync(id, adminUserId, request);
            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WordfinderCatalogueDto>> Reject(int id, [FromBody] RejectWordfinderCatalogueRequest request)
        {
            var adminUserId = _userContextService.GetUserId!.Value;
            var result = await _wordfinderService.RejectCatalogueAsync(id, adminUserId, request);
            return Ok(result);
        }
    }
}
