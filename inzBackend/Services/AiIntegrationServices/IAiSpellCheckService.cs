using inzBackend.Models.AiSpellCheckingModels;

namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiSpellCheckService
    {
        Task<SpellCheckResult> checkTextAsync(string text, string language = "English");
        Task<List<SpellCheckResult>> checkBatchAsync(List<(string Text, string Language)> items);
    }
}
