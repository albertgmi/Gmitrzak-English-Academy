using inzBackend.Models.CatalogueModels;
using inzBackend.Services.CatalogueServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/catalogue")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CatalogueController : ControllerBase
    {
        private readonly ICatalogueService _catalogueService;

        public CatalogueController(ICatalogueService catalogueService)
        {
            _catalogueService = catalogueService;
        }

        [HttpGet]
        public ActionResult<List<CatalogueDto>> getAll()
        {
            return _catalogueService.getAllCatalogues();
        }
            
        [HttpPost("upload")]
        public async Task<CatalogueDto> upload(IFormFile file)
        {
            return await _catalogueService.uploadCatalogue(file);
        }

        [HttpGet("entries")]
        public ActionResult<List<CatalogueEntryDto>> getEntries([FromQuery] CatalogueEntryFilterRequest filter)
        {
            return _catalogueService.getEntries(filter);
        }

        [HttpDelete("{catalogueId}")]
        public ActionResult delete(int catalogueId)
        {
            _catalogueService.deleteCatalogue(catalogueId);
            return NoContent();
        }

        [HttpPut("entries/{entryId}")]
        public ActionResult updateTranslation([FromBody] UpdateTranslationRequest request, [FromRoute] int entryId)
        {
            _catalogueService.updateTranslation(request, entryId);
            return Ok();
        }
    }
}
