using inzBackend.Models.AIAnswerCheckingModels;

namespace inzBackend.Services.UserAnswerServices
{
    public interface IUserAnswerService
    {
        Task<AnswerResultDto> submitAnswerAsync(int userId, SubmitAnswerRequest request);
        List<AnswerResultDto> getAnswersForAssignment(int assignmentId);
        void overrideAnswer(int answerId, TeacherOverrideRequest request);
    }
}
