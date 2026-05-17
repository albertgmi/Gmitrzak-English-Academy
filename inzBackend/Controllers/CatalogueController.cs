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
        public List<CatalogueDto> getAll() =>
            _catalogueService.getAllCatalogues();

        [HttpPost("upload")]
        public async Task<CatalogueDto> upload(IFormFile file) =>
            await _catalogueService.uploadCatalogue(file);

        [HttpGet("entries")]
        public List<CatalogueEntryDto> getEntries(
            [FromQuery] string? catalogueName,
            [FromQuery] string? userRef,
            [FromQuery] DateOnly? dateFrom,
            [FromQuery] DateOnly? dateTo)
        {
            return _catalogueService.getEntries(new CatalogueEntryFilterRequest
            {
                CatalogueName = catalogueName,
                UserRef = userRef,
                DateFrom = dateFrom,
                DateTo = dateTo
            });
        }

        [HttpDelete("{catalogueId}")]
        public IActionResult delete(int catalogueId)
        {
            _catalogueService.deleteCatalogue(catalogueId);
            return NoContent();
        }
    }
}
