using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Services.SentenceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Controllers
{
    [Route("api/sentence")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SentenceController : ControllerBase
    {
        private readonly ISentenceService _sentenceService;

        public SentenceController(ISentenceService sentenceService)
        {
            _sentenceService = sentenceService;
        }

        [HttpGet("stock")]
        public ActionResult<List<SentenceStockDto>> GetStock()
        {
            return _sentenceService.GetAllStock();
        }

        [HttpPost("stock")]
        public ActionResult CreateStock([FromBody] CreateSentenceStockRequest request)
        {
            _sentenceService.CreateStock(request);
            return Ok();
        }

        [HttpDelete("stock/{id}")]
        public ActionResult DeleteStock([FromRoute] int id)
        {
            _sentenceService.DeleteStock(id);
            return NoContent();
        }

        [HttpPost("stock/upload")]
        public async Task<ActionResult> UploadStock(IFormFile file)
        {
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { message = "Only .xlsx and .xls files are allowed" });

            var count = await _sentenceService.UploadStockFromExcel(file);
            return Ok(new { added = count });
        }

        [HttpGet("sets")]
        public ActionResult<List<SentenceSetGroupDto>> GetSets()
        {
            return _sentenceService.GetAllSetsGrouped();
        }

        [HttpGet("sets/{id}")]
        public ActionResult<SentenceSetDto> GetSet([FromRoute] int id)
        {
            return _sentenceService.GetSet(id);
        }

        [HttpPost("sets")]
        public ActionResult<SentenceSetDto> CreateSet([FromBody] CreateSentenceSetRequest request)
        {
            return _sentenceService.CreateSet(request);
        }

        [HttpDelete("sets/{id}")]
        public ActionResult DeleteSet([FromRoute] int id)
        {
            _sentenceService.DeleteSet(id);
            return NoContent();
        }

        [HttpPost("assign-to-module")]
        public ActionResult AssignToModule([FromBody] AssignSetToModuleRequest request)
        {
            _sentenceService.AssignToModule(request);
            return Ok();
        }

        [HttpGet("module/{moduleId}/sets")]
        public ActionResult<List<SentenceSetDto>> GetSetsForModule([FromRoute] int moduleId)
        {
            return _sentenceService.GetSetsForModule(moduleId);
        }

        [HttpDelete("module/{moduleId}/set/{setId}")]
        public ActionResult RemoveSetFromModule([FromRoute] int moduleId, [FromRoute] int setId)
        {
            _sentenceService.RemoveSetFromModule(moduleId, setId);
            return NoContent();
        }

        [HttpPut("stock/{id}")]
        public ActionResult UpdateStock([FromRoute] int id, [FromBody] UpdateSentenceStockRequest request)
        {
            _sentenceService.UpdateStock(id, request);
            return Ok();
        }

        [HttpPost("assign")]
        public ActionResult AssignToUser([FromBody] AssignSentenceRequest request)
        {
            _sentenceService.AssignToUser(request);
            return Ok();
        }

        [HttpGet("search")]
        public async Task<ActionResult<SearchSentenceResultDto>> SearchSentence([FromQuery] string query, [FromQuery] int studentId)
        {
            var result = await _sentenceService.SearchSentence(query, studentId);
            return Ok(result);
        }

        [HttpPost("assign-set")]
        public ActionResult AssignSentenceSet([FromBody] AssignSentenceSetToStudentRequest request)
        {
            _sentenceService.AssignSentenceSetToUser(request);
            return Ok();
        }
    }
}