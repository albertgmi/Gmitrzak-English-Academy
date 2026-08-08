namespace inzBackend.Services.AiIntegrationServices
{
    public interface IAiUsageGuardService
    {
        void EnsureCanSubmitAttempt(int userId);
    }
}
