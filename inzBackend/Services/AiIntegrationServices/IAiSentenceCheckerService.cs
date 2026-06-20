using inzBackend.Models.AIAnswerCheckingModels;

namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiSentenceCheckerService
    {
        Task<SentenceCheckResult> CheckAnswerAsync(string polish, string expectedEnglish, string userAnswer);
    }
}
