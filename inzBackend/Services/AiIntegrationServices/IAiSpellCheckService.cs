using inzBackend.Models.AiSpellCheckingModels;

namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiSpellCheckService
    {
        Task<SpellCheckResult> CheckTextAsync(string text, string language = "English");
        Task<List<SpellCheckResult>> CheckBatchAsync(List<SpellCheckRequestItem> items);
    }
}
