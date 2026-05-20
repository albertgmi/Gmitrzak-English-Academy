using inzBackend.Entities;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;
using inzBackend.Models;
using inzBackend.Services.AiIntegrationServices;
using inzBackend.Services.SentenceServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using inzBackend.Services.UserAnswerServices;

[Route("api/sentence")]
[ApiController]
[Authorize(Roles = "Admin")]
public class SentenceController : ControllerBase
{
    private readonly ISentenceService _sentenceService;
    private readonly IUserAnswerService _userAnswerService;
    private readonly IUserContextService _userContextService;

    public SentenceController(
        ISentenceService sentenceService,
        IUserAnswerService userAnswerService,
        IUserContextService userContextService)
    {
        _sentenceService = sentenceService;
        _userAnswerService = userAnswerService;
        _userContextService = userContextService;
    }

    [HttpGet("stock")]
    public List<SentenceStockDto> getStock() => _sentenceService.getAllStock();

    [HttpPost("stock")]
    public IActionResult createStock([FromBody] CreateSentenceStockRequest request)
    {
        _sentenceService.createStock(request);
        return Ok();
    }

    [HttpDelete("stock/{id}")]
    public IActionResult deleteStock(int id)
    {
        _sentenceService.deleteStock(id);
        return NoContent();
    }

    [HttpPost("stock/upload")]
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
    public List<SentenceSetGroupDto> getSets() => _sentenceService.getAllSetsGrouped();

    [HttpGet("sets/{id}")]
    public SentenceSetDto getSet(int id) => _sentenceService.getSet(id);

    [HttpPost("sets")]
    public SentenceSetDto createSet([FromBody] CreateSentenceSetRequest request) =>
        _sentenceService.createSet(request);

    [HttpDelete("sets/{id}")]
    public IActionResult deleteSet(int id)
    {
        _sentenceService.deleteSet(id);
        return NoContent();
    }

    [HttpPost("assign")]
    public IActionResult assign([FromBody] AssignSentenceRequest request)
    {
        _sentenceService.assignToUser(request);
        return Ok();
    }

    [HttpPost("answer")]
    [Authorize]
    public async Task<ActionResult<AnswerResultDto>> submitAnswer([FromBody] SubmitAnswerRequest request)
    {
        var userId = _userContextService.GetUserId!.Value;
        var result = await _userAnswerService.submitAnswerAsync(userId, request);
        return Ok(result);
    }

    [HttpGet("answers/{assignmentId}")]
    public ActionResult<List<AnswerResultDto>> getAnswers(int assignmentId)
    {
        var results = _userAnswerService.getAnswersForAssignment(assignmentId);
        return Ok(results);
    }

    [HttpPatch("answers/{answerId}/override")]
    public IActionResult overrideAnswer(int answerId, [FromBody] TeacherOverrideRequest request)
    {
        _userAnswerService.overrideAnswer(answerId, request);
        return Ok();
    }
}