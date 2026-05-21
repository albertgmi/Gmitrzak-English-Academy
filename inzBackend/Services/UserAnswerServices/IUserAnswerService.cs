using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.ModuleReportModels;

namespace inzBackend.Services.UserAnswerServices
{
    public interface IUserAnswerService
    {
        Task<AnswerResultDto> submitAnswerAsync(SubmitAnswerRequest request);
        List<AnswerResultDto> getAnswersForModule(int moduleId);
        List<AnswerResultDto> getAnswersForModuleByStudent(int moduleId, int studentId);
        void overrideAnswer(int answerId, TeacherOverrideRequest request);
        ModuleReportDto generateReport(int moduleId, int studentId);
    }
}
