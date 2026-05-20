namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiSentenceCheckerService
    {
        Task<(string result, string explanation)> CheckAnswerAsync(
            string polish, string expectedEnglish, string userAnswer);
    }
}
