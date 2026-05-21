using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Services.SentenceServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/sentence")]
[ApiController]
public class SentenceController : ControllerBase
{
    private readonly ISentenceService _sentenceService;

    public SentenceController(ISentenceService sentenceService)
    {
        _sentenceService = sentenceService;
    }

    [HttpGet("stock")]
    [Authorize(Roles = "Admin")]
    public List<SentenceStockDto> getStock() =>
        _sentenceService.getAllStock();

    [HttpPost("stock")]
    [Authorize(Roles = "Admin")]
    public IActionResult createStock([FromBody] CreateSentenceStockRequest request)
    {
        _sentenceService.createStock(request);
        return Ok();
    }

    [HttpDelete("stock/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult deleteStock(int id)
    {
        _sentenceService.deleteStock(id);
        return NoContent();
    }

    [HttpPost("stock/upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> uploadStock(IFormFile file)
    {
        var allowedExtensions = new[] { ".xlsx", ".xls" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(ext))
            return BadRequest(new { message = "Only .xlsx and .xls files are allowed" });

        var count = await _sentenceService.uploadStockFromExcel(file);
        return Ok(new { added = count });
    }

    [HttpGet("sets")]
    [Authorize(Roles = "Admin")]
    public List<SentenceSetGroupDto> getSets() =>
        _sentenceService.getAllSetsGrouped();

    [HttpGet("sets/{id}")]
    [Authorize(Roles = "Admin")]
    public SentenceSetDto getSet(int id) =>
        _sentenceService.getSet(id);

    [HttpPost("sets")]
    [Authorize(Roles = "Admin")]
    public SentenceSetDto createSet([FromBody] CreateSentenceSetRequest request) =>
        _sentenceService.createSet(request);

    [HttpDelete("sets/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult deleteSet(int id)
    {
        _sentenceService.deleteSet(id);
        return NoContent();
    }

    [HttpPost("assign-to-module")]
    [Authorize(Roles = "Admin")]
    public IActionResult assignToModule([FromBody] AssignSetToModuleRequest request)
    {
        _sentenceService.assignToModule(request);
        return Ok();
    }

    [HttpGet("module/{moduleId}/sets")]
    [Authorize(Roles = "Admin")]
    public List<SentenceSetDto> getSetsForModule(int moduleId) =>
        _sentenceService.getSetsForModule(moduleId);

    [HttpDelete("module/{moduleId}/set/{setId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult removeSetFromModule(int moduleId, int setId)
    {
        _sentenceService.removeSetFromModule(moduleId, setId);
        return NoContent();
    }
}