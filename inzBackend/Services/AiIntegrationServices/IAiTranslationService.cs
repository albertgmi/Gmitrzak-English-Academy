namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiTranslationService
    {
        Task<List<string>> TranslateBatchAsync(List<string> texts, string targetLanguage = "Polish");
        Task<List<bool>> ValidateTranslationsAsync(List<(string Source, string Translation)> pairs, string targetLanguage = "Polish");
    }
}
