using inzBackend.Models.AiPronunciationModels;

namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiPronunciationService
    {
        Task<PronunciationResult> processUserAttemptAsync(Stream audioStream, string fileName, int pronunciationEntryId);
    }
}
