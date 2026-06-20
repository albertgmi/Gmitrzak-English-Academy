using inzBackend.Models.AiPronunciationModels;

namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiPronunciationService
    {
        Task<PronunciationResult> ProcessUserAttemptAsync(Stream audioStream, string fileName, int pronunciationEntryId);
    }
}
