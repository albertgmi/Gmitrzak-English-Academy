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
        List<inzBackend.Models.SentenceModels.SentenceModuleLiveDto> GetSentenceModulesForLiveRoom(int? studentId = null);
        List<inzBackend.Models.SentenceModels.SentenceAnswerLiveDto> GetSentenceAnswersForLiveRoom(int moduleId, int? studentId = null);
        inzBackend.Models.SentenceModels.SentenceAnswerLiveDto GetSentenceAnswerDetailForLiveRoom(int answerId);
        inzBackend.Models.SentenceModels.SentenceAnswerLiveDto SaveSentenceReview(int answerId, inzBackend.Models.SentenceModels.SaveSentenceReviewRequest request);
        List<inzBackend.Models.SentenceModels.SentenceAnswerCommentDto> GetCommentsForSentenceAnswer(int answerId);
        inzBackend.Models.SentenceModels.SentenceAnswerCommentDto AddCommentToSentenceAnswer(int answerId, inzBackend.Models.SentenceModels.CreateSentenceAnswerCommentRequest request);
        void ArchiveSentenceAnswerComment(int commentId);
    }
}
