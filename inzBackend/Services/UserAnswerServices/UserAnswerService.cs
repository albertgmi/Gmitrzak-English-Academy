using DocumentFormat.OpenXml.Office2016.Excel;
using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.ModuleReportModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.AiIntegrationServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace inzBackend.Services.UserAnswerServices
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IAiSentenceCheckerService _aiService;
        private readonly IUserContextService _userContextService;
        private readonly ILessonPanelService _lessonPanelService;

        public UserAnswerService(GmitrzakEnglishAcademyDbContext dbContext, IAiSentenceCheckerService aiService,
            IUserContextService userContextService, ILessonPanelService lessonPanelService)
        {
            _dbContext = dbContext;
            _aiService = aiService;
            _userContextService = userContextService;
            _lessonPanelService = lessonPanelService;
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

            var isCorrectOrPartial = result == "Correct" || result == "Partial";

            if (!alreadyExists)
            {
                _dbContext.Sentences.Add(new Sentence
                {
                    UserId = userId,
                    Content = sentence.Polish,
                    Translation = result == "Correct" || result == "Partial"
                        ? request.UserAnswer
                        : sentence.EnglishTranslation
                });
            }

            if(!isCorrectOrPartial && !alreadyExists)
            {
                _dbContext.Sentences.Add(new Sentence
                {
                    UserId = userId,
                    Content = sentence.Polish,
                    Translation = sentence.EnglishTranslation,
                    NextReviewDate = PolandTime.Today
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
                GeneratedDate = PolandTime.Today,
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
                .CountAsync(x => x.UserId == userId && x.ModuleId == moduleId);

            if (answeredCount < totalSentences) return;

            var directAssignment = await _dbContext.UserModuleAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId
                                       && x.ModuleId == moduleId);

            if (directAssignment is not null && !directAssignment.IsCompleted)
            {
                await assignModuleCompletionPointsAsync(userId, moduleId, directAssignment);
                directAssignment.IsCompleted = true;
                await _dbContext.SaveChangesAsync();
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
                    var matrixModule = await _dbContext.MatrixModules
                        .Include(x => x.Matrix)
                        .FirstOrDefaultAsync(x => x.Id == mmId);

                    var matrixAssignment = matrixModule is not null
                        ? await _dbContext.UserMatrixAssignments
                            .FirstOrDefaultAsync(x => x.UserId == userId
                                                   && x.MatrixId == matrixModule.MatrixId)
                        : null;

                    if (matrixAssignment is not null && matrixModule is not null)
                    {
                        var unlockDate = matrixAssignment.StartDate
                            .AddDays((matrixModule.WeekNumber - 1) * matrixAssignment.Matrix.RefreshIntervalDays)
                            .AddDays(matrixModule.DayOfWeek - 1);

                        var dueDate = unlockDate.AddDays(7);
                        var fakeAssignment = new UserModuleAssignment
                        {
                            DueDate = dueDate,
                            IsCompleted = false
                        };
                        await assignModuleCompletionPointsAsync(userId, moduleId, fakeAssignment);
                    }

                    _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
                    {
                        UserId = userId,
                        MatrixModuleId = mmId,
                        CompletedDate = PolandTime.Today
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task assignModuleCompletionPointsAsync(int userId, int moduleId, UserModuleAssignment? assignment)
        {
            var answers = await _dbContext.UserSentenceAnswers
                .Where(x => x.UserId == userId && x.ModuleId == moduleId)
                .ToListAsync();

            var module = await _dbContext.Modules
                .FirstOrDefaultAsync(x => x.Id == moduleId);
            var moduleName = module?.Name ?? "Unknown Module";

            var today = PolandTime.Today;

            int totalPoints = answers.Sum(ans => (ans.TeacherOverride ?? ans.AiResult) switch
            {
                "Correct" => 5,
                "Partial" => 3,
                _ => 1
            });

            int bonus = 0;
            if (assignment is not null && today <= assignment.DueDate)
                bonus = 10;

            totalPoints += bonus;

            string reason = bonus > 0
                ? $"Module '{moduleName}' completed (+{bonus} bonus for on-time)"
                : $"Module '{moduleName}' completed (no bonus — past due date)";

            _lessonPanelService.addActivityPoints(userId, totalPoints, reason);
        }
    }
}