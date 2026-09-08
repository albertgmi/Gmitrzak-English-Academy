using inzBackend.Enums;
using inzBackend.Models.AiSpellCheckingModels;
using inzBackend.Models.WordfinderModels;

namespace inzBackend.Services.WordfinderServices
{
    public interface IWordfinderService
    {
        // Student actions
        Task<WordfinderCatalogueDto> CreateDraftAsync(int studentUserId, CreateWordfinderCatalogueDto dto);
        Task<List<WordfinderCatalogueListDto>> GetMyCataloguesAsync(int studentUserId);
        Task<WordfinderCatalogueDto> GetByIdAsync(int id, int userId, bool isAdmin);
        Task<WordfinderCatalogueDto> UpdateDraftAsync(int id, int studentUserId, UpdateWordfinderCatalogueDto dto);
        Task DeleteDraftAsync(int id, int studentUserId);
        Task<WordfinderCatalogueDto> SubmitAsync(int id, int studentUserId);
        Task<TranslateWordfinderEntryResponse> TranslateEntryAsync(TranslateWordfinderEntryRequest request);
        Task<SpellCheckResult> SpellCheckEntryAsync(SpellCheckRequest request);

        // Admin actions
        Task<List<WordfinderCatalogueListDto>> GetPendingCataloguesAsync(WordfinderCatalogueStatus? statusFilter = null);
        Task<WordfinderCatalogueDto> ApproveCatalogueAsync(int id, int adminUserId, ApproveWordfinderCatalogueRequest? request);
        Task<WordfinderCatalogueDto> RejectCatalogueAsync(int id, int adminUserId, RejectWordfinderCatalogueRequest request);
    }
}
