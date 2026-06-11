using inzBackend.Models.CatalogueModels;

namespace inzBackend.Services.CatalogueServices
{
    public interface ICatalogueService
    {
        Task<CatalogueDto> uploadCatalogue(IFormFile file);
        List<CatalogueDto> getAllCatalogues();
        List<CatalogueEntryDto> getEntries(CatalogueEntryFilterRequest filter);
        void deleteCatalogue(int catalogueId);
        void updateTranslation(UpdateTranslationRequest request, int entryId);
    }
}
