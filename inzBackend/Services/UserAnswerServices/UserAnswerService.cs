using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models;
using inzBackend.Services.AiIntegrationServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.UserAnswerServices
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IAiSentenceCheckerService _aiService;

        public UserAnswerService(GmitrzakEnglishAcademyDbContext dbContext, IAiSentenceCheckerService aiService)
        {
            _dbContext = dbContext;
            _aiService = aiService;
        }

        public async Task<AnswerResultDto> submitAnswerAsync(int userId, SubmitAnswerRequest request)
        {
            var sentence = _dbContext.SentenceStocks.FirstOrDefault(x => x.Id == request.SentenceStockId)
                ?? throw new NotFoundException("Sentence not found");

            var checkedResult = await _aiService.CheckAnswerAsync(
                sentence.Polish, sentence.EnglishTranslation, request.UserAnswer);

            var answer = new UserSentenceAnswer
            {
                UserId = userId,
                AssignmentId = request.AssignmentId,
                SentenceStockId = request.SentenceStockId,
                UserAnswer = request.UserAnswer,
                AiResult = checkedResult.result,
                AiExplanation = checkedResult.explanation,
                TeacherReviewed = false
            };

            _dbContext.UserSentenceAnswers.Add(answer);
            await _dbContext.SaveChangesAsync();

            return new AnswerResultDto
            {
                Id = answer.Id,
                Polish = sentence.Polish,
                ExpectedTranslation = sentence.EnglishTranslation,
                UserAnswer = request.UserAnswer,
                AiResult = checkedResult.result,
                AiExplanation = checkedResult.explanation
            };
        }

        public List<AnswerResultDto> getAnswersForAssignment(int assignmentId)
        {
            return _dbContext.UserSentenceAnswers
                .Include(x => x.SentenceStock)
                .Where(x => x.AssignmentId == assignmentId)
                .Select(x => new AnswerResultDto
                {
                    Id = x.Id,
                    Polish = x.SentenceStock.Polish,
                    ExpectedTranslation = x.SentenceStock.EnglishTranslation,
                    UserAnswer = x.UserAnswer,
                    AiResult = x.TeacherOverride ?? x.AiResult,
                    AiExplanation = x.AiExplanation,
                    TeacherOverride = x.TeacherOverride,
                    TeacherReviewed = x.TeacherReviewed
                })
                .ToList();
        }

        public void overrideAnswer(int answerId, TeacherOverrideRequest request)
        {
            var answer = _dbContext
                .UserSentenceAnswers
                .FirstOrDefault(x => x.Id == answerId)
                ?? throw new NotFoundException("Answer not found");

            answer.TeacherOverride = request.Override;
            answer.TeacherReviewed = true;
            _dbContext.SaveChanges();
        }
    }
}
