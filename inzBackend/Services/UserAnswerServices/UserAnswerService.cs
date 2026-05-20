using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models;
using inzBackend.Services.AiIntegrationServices;
using Microsoft.EntityFrameworkCore;
using inzBackend.Services.UserServices;

namespace inzBackend.Services.UserAnswerServices
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IAiSentenceCheckerService _aiService;
        private readonly IUserContextService _userContextService;

        public UserAnswerService(GmitrzakEnglishAcademyDbContext dbContext, IAiSentenceCheckerService aiService,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _aiService = aiService;
            _userContextService = userContextService;
        }

        public async Task<AnswerResultDto> submitAnswerAsync(SubmitAnswerRequest request)
        {
            var userId = _userContextService.GetUserId!.Value;
            var sentence = await _dbContext.SentenceStocks
                .FirstOrDefaultAsync(x => x.Id == request.SentenceStockId)
                ?? throw new NotFoundException("Sentence not found");

            var (result, explanation) = await _aiService.CheckAnswerAsync(
                sentence.Polish, sentence.EnglishTranslation, request.UserAnswer);

            var answer = new UserSentenceAnswer
            {
                UserId = userId,
                AssignmentId = request.AssignmentId,
                SentenceStockId = request.SentenceStockId,
                UserAnswer = request.UserAnswer,
                AiResult = result,
                AiExplanation = explanation,
                TeacherReviewed = false
            };

            _dbContext.UserSentenceAnswers.Add(answer);

            if (result is "Correct" or "Partial")
            {
                var alreadyExists = await _dbContext.Sentences
                    .AnyAsync(x => x.UserId == userId && x.Content == sentence.Polish);

                if (!alreadyExists)
                {
                    _dbContext.Sentences.Add(new Sentence
                    {
                        UserId = userId,
                        Content = sentence.Polish,
                        Translation = request.UserAnswer,
                        Notes = $"AI: {result}"
                    });
                }
            }

            await _dbContext.SaveChangesAsync();

            return new AnswerResultDto
            {
                Id = answer.Id,
                Polish = sentence.Polish,
                ExpectedTranslation = sentence.EnglishTranslation,
                UserAnswer = request.UserAnswer,
                AiResult = result,
                AiExplanation = explanation
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