using inzBackend.Models.StudentLearningModels.AlphabetModels;

namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiAlphabetService
    {
        Task<AlphabetResult> ProcessUserAttemptAsync(Stream audioStream, string fileName, int alphabetEntryId);
    }
}
