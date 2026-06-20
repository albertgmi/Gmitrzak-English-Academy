using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.ModuleReportModels;

namespace inzBackend.Services.UserAnswerServices
{
    public interface IUserAnswerService
    {
        Task<AnswerResultDto> SubmitAnswerAsync(SubmitAnswerRequest request);
        List<AnswerResultDto> GetAnswersForModule(int moduleId);
        List<AnswerResultDto> GetAnswersForModuleByStudent(int moduleId, int studentId);
        void OverrideAnswer(int answerId, TeacherOverrideRequest request);
        ModuleReportDto GenerateReport(int moduleId, int studentId);
        List<CompletedSentenceModuleDto> GetCompletedSentenceModules(int studentId, DateOnly dateFrom, DateOnly dateTo);
        DateRangeReportDto GenerateDateRangeReport(int studentId, DateOnly dateFrom, DateOnly dateTo);
    }
}
