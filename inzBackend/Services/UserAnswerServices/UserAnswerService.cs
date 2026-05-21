using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.ModuleReportModels;
using inzBackend.Services.AiIntegrationServices;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.UserAnswerServices
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IAiSentenceCheckerService _aiService;
        private readonly IUserContextService _userContextService;

        public UserAnswerService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IAiSentenceCheckerService aiService,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _aiService = aiService;
            _userContextService = userContextService;
        }

        public async Task<AnswerResultDto> submitAnswerAsync(SubmitAnswerRequest request)
        {
            var userId = _userContextService.GetUserId!.Value;

            var module = await _dbContext.Modules
                .FirstOrDefaultAsync(x => x.Id == request.ModuleId)
                ?? throw new NotFoundException($"Module {request.ModuleId} not found");

            var sentence = await _dbContext.SentenceStocks
                .FirstOrDefaultAsync(x => x.Id == request.SentenceStockId)
                ?? throw new NotFoundException("Sentence not found");

            var (result, explanation) = await _aiService.CheckAnswerAsync(
                sentence.Polish, sentence.EnglishTranslation, request.UserAnswer);

            var existingAnswer = await _dbContext.UserSentenceAnswers
                .FirstOrDefaultAsync(x => x.UserId == userId
                                       && x.ModuleId == request.ModuleId
                                       && x.SentenceStockId == request.SentenceStockId);

            if (existingAnswer is not null)
            {
                existingAnswer.UserAnswer = request.UserAnswer;
                existingAnswer.AiResult = result;
                existingAnswer.AiExplanation = explanation;
                existingAnswer.TeacherOverride = null;
                existingAnswer.TeacherExplanation = null;
                existingAnswer.TeacherReviewed = false;
            }
            else
            {
                existingAnswer = new UserSentenceAnswer
                {
                    UserId = userId,
                    ModuleId = request.ModuleId,
                    SentenceStockId = request.SentenceStockId,
                    UserAnswer = request.UserAnswer,
                    AiResult = result,
                    AiExplanation = explanation,
                    TeacherReviewed = false
                };
                _dbContext.UserSentenceAnswers.Add(existingAnswer);
            }

            var alreadyExists = await _dbContext.Sentences
                .AnyAsync(x => x.UserId == userId && x.Content == sentence.Polish);

            if (!alreadyExists)
            {
                _dbContext.Sentences.Add(new Sentence
                {
                    UserId = userId,
                    Content = sentence.Polish,
                    Translation = result == "Correct" || result == "Partial"
                        ? request.UserAnswer
                        : sentence.EnglishTranslation,
                    Notes = $"AI: {result}"
                });
            }

            await _dbContext.SaveChangesAsync();

            await tryCompleteModuleAsync(userId, request.ModuleId);
            await _dbContext.SaveChangesAsync();

            return new AnswerResultDto
            {
                Id = existingAnswer.Id,
                Polish = sentence.Polish,
                ExpectedTranslation = sentence.EnglishTranslation,
                UserAnswer = request.UserAnswer,
                AiResult = result,
                AiExplanation = explanation
            };
        }

        public List<AnswerResultDto> getAnswersForModule(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;
            return mapAnswers(moduleId, userId);
        }

        public List<AnswerResultDto> getAnswersForModuleByStudent(int moduleId, int studentId)
        {
            return mapAnswers(moduleId, studentId);
        }

        public void overrideAnswer(int answerId, TeacherOverrideRequest request)
        {
            var answer = _dbContext.UserSentenceAnswers
                .FirstOrDefault(x => x.Id == answerId)
                ?? throw new NotFoundException("Answer not found");

            answer.TeacherOverride = request.Override;
            answer.TeacherExplanation = request.TeacherExplanation;
            answer.TeacherReviewed = true;
            _dbContext.SaveChanges();
        }

        public ModuleReportDto generateReport(int moduleId, int studentId)
        {
            var module = _dbContext.Modules
                .FirstOrDefault(x => x.Id == moduleId)
                ?? throw new NotFoundException("Module not found");

            var student = _dbContext.Users
                .FirstOrDefault(x => x.Id == studentId)
                ?? throw new NotFoundException("Student not found");

            var setIds = _dbContext.ModuleSentenceSets
                .Where(x => x.ModuleId == moduleId)
                .Select(x => x.SentenceSetId)
                .ToList();

            var sentenceItems = _dbContext.SentenceSetItems
                .Include(x => x.SentenceStock)
                .Where(x => setIds.Contains(x.SentenceSetId))
                .OrderBy(x => x.SentenceSetId).ThenBy(x => x.Order)
                .ToList();

            var answers = _dbContext.UserSentenceAnswers
                .Where(x => x.UserId == studentId && x.ModuleId == moduleId)
                .ToDictionary(x => x.SentenceStockId);

            var items = sentenceItems.Select((item, idx) =>
            {
                answers.TryGetValue(item.SentenceStockId, out var ans);
                var finalResult = ans == null ? "Not answered"
                    : (ans.TeacherOverride ?? ans.AiResult);

                return new ModuleReportItemDto
                {
                    Order = idx + 1,
                    Polish = item.SentenceStock.Polish,
                    ExpectedTranslation = item.SentenceStock.EnglishTranslation,
                    StudentAnswer = ans?.UserAnswer ?? "—",
                    AiResult = ans?.AiResult ?? "—",
                    AiExplanation = ans?.AiExplanation ?? "—",
                    TeacherOverride = ans?.TeacherOverride,
                    TeacherExplanation = ans?.TeacherExplanation,
                    FinalResult = finalResult
                };
            }).ToList();

            return new ModuleReportDto
            {
                ModuleName = module.Name,
                StudentUsername = student.Username,
                GeneratedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalSentences = items.Count,
                CorrectCount = items.Count(x => x.FinalResult == "Correct"),
                PartialCount = items.Count(x => x.FinalResult == "Partial"),
                IncorrectCount = items.Count(x => x.FinalResult == "Incorrect"),
                Items = items
            };
        }

        private List<AnswerResultDto> mapAnswers(int moduleId, int userId)
        {
            return _dbContext.UserSentenceAnswers
                .Include(x => x.SentenceStock)
                .Where(x => x.UserId == userId && x.ModuleId == moduleId)
                .Select(x => new AnswerResultDto
                {
                    Id = x.Id,
                    Polish = x.SentenceStock.Polish,
                    ExpectedTranslation = x.SentenceStock.EnglishTranslation,
                    UserAnswer = x.UserAnswer,
                    AiResult = x.AiResult,
                    AiExplanation = x.AiExplanation,
                    TeacherOverride = x.TeacherOverride,
                    TeacherExplanation = x.TeacherExplanation,
                    TeacherReviewed = x.TeacherReviewed
                })
                .ToList();
        }

        private async Task tryCompleteModuleAsync(int userId, int moduleId)
        {
            var totalSentences = await _dbContext.ModuleSentenceSets
                .Include(x => x.SentenceSet).ThenInclude(s => s.Items)
                .Where(x => x.ModuleId == moduleId)
                .SelectMany(x => x.SentenceSet.Items)
                .CountAsync();

            if (totalSentences == 0) return;

            var answeredCount = await _dbContext.UserSentenceAnswers
                .CountAsync(x => x.UserId == userId
                              && x.ModuleId == moduleId);

            if (answeredCount < totalSentences) return;

            var directAssignment = await _dbContext.UserModuleAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId
                                       && x.ModuleId == moduleId
                                       && !x.IsCompleted);

            if (directAssignment is not null)
            {
                directAssignment.IsCompleted = true;
                return;
            }

            var matrixModules = await _dbContext.MatrixModules
                .Where(x => x.ModuleId == moduleId)
                .Select(x => x.Id)
                .ToListAsync();

            foreach (var mmId in matrixModules)
            {
                var alreadyCompleted = await _dbContext.UserMatrixModuleCompletions
                    .AnyAsync(x => x.UserId == userId && x.MatrixModuleId == mmId);

                if (!alreadyCompleted)
                {
                    _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
                    {
                        UserId = userId,
                        MatrixModuleId = mmId,
                        CompletedDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    });
                }
            }
        }
    }
}