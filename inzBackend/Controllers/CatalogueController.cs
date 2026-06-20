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
        public ActionResult<List<CatalogueDto>> GetAll()
        {
            return _catalogueService.GetAllCatalogues();
        }
            
        [HttpPost("upload")]
        public async Task<CatalogueDto> Upload(IFormFile file)
        {
            return await _catalogueService.UploadCatalogue(file);
        }

        [HttpGet("entries")]
        public ActionResult<List<CatalogueEntryDto>> GetEntries([FromQuery] CatalogueEntryFilterRequest filter)
        {
            return _catalogueService.GetEntries(filter);
        }

        [HttpDelete("{catalogueId}")]
        public ActionResult Delete(int catalogueId)
        {
            _catalogueService.DeleteCatalogue(catalogueId);
            return NoContent();
        }

        [HttpPut("entries/{entryId}")]
        public ActionResult UpdateTranslation([FromBody] UpdateTranslationRequest request, [FromRoute] int entryId)
        {
            _catalogueService.UpdateTranslation(request, entryId);
            return Ok();
        }
    }
}
