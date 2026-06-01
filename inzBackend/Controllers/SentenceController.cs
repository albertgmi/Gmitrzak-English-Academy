using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Services.SentenceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public List<SentenceStockDto> getStock()
        {
            return _sentenceService.getAllStock();
        }

        [HttpPost("stock")]
        public ActionResult createStock([FromBody] CreateSentenceStockRequest request)
        {
            _sentenceService.createStock(request);
            return Ok();
        }

        [HttpDelete("stock/{id}")]
        public ActionResult deleteStock([FromRoute] int id)
        {
            _sentenceService.deleteStock(id);
            return NoContent();
        }

        [HttpPost("stock/upload")]
        public async Task<ActionResult> uploadStock(IFormFile file)
        {
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { message = "Only .xlsx and .xls files are allowed" });

            var count = await _sentenceService.uploadStockFromExcel(file);
            return Ok(new { added = count });
        }

        [HttpGet("sets")]
        public List<SentenceSetGroupDto> getSets()
        {
            return _sentenceService.getAllSetsGrouped();
        }

        [HttpGet("sets/{id}")]
        public SentenceSetDto getSet([FromRoute] int id)
        {
            return _sentenceService.getSet(id);
        }

        [HttpPost("sets")]
        public SentenceSetDto createSet([FromBody] CreateSentenceSetRequest request)
        {
            return _sentenceService.createSet(request);
        }

        [HttpDelete("sets/{id}")]
        public ActionResult deleteSet([FromRoute] int id)
        {
            _sentenceService.deleteSet(id);
            return NoContent();
        }

        [HttpPost("assign-to-module")]
        public ActionResult assignToModule([FromBody] AssignSetToModuleRequest request)
        {
            _sentenceService.assignToModule(request);
            return Ok();
        }

        [HttpGet("module/{moduleId}/sets")]
        public List<SentenceSetDto> getSetsForModule([FromRoute] int moduleId)
        {
            return _sentenceService.getSetsForModule(moduleId);
        }

        [HttpDelete("module/{moduleId}/set/{setId}")]
        public ActionResult removeSetFromModule([FromRoute] int moduleId, [FromRoute] int setId)
        {
            _sentenceService.removeSetFromModule(moduleId, setId);
            return NoContent();
        }

        [HttpPut("stock/{id}")]
        public ActionResult updateStock([FromRoute] int id, [FromBody] UpdateSentenceStockRequest request)
        {
            _sentenceService.updateStock(id, request);
            return Ok();
        }
    }
}