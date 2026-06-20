using inzBackend.Models.CatalogueModels;

namespace inzBackend.Services.CatalogueServices
{
    public interface ICatalogueService
    {
        Task<CatalogueDto> UploadCatalogue(IFormFile file);
        List<CatalogueDto> GetAllCatalogues();
        List<CatalogueEntryDto> GetEntries(CatalogueEntryFilterRequest filter);
        void DeleteCatalogue(int catalogueId);
        void UpdateTranslation(UpdateTranslationRequest request, int entryId);
    }
}
