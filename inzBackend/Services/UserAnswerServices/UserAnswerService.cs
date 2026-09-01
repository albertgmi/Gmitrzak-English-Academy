using DocumentFormat.OpenXml.Office2016.Excel;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.LearningMaterials;
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

        public async Task<AnswerResultDto> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var userId = _userContextService.GetUserId!.Value;

            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.Id == userId);

            var module = await _dbContext.Modules
                .FirstOrDefaultAsync(x => x.Id == request.ModuleId)
                ?? throw new NotFoundException($"Module {request.ModuleId} not found");

            var sentence = await _dbContext.SentenceStocks
                .FirstOrDefaultAsync(x => x.Id == request.SentenceStockId)
                ?? throw new NotFoundException("Sentence not found");

            var sentenceCheckResult = await _aiService.CheckAnswerAsync(
                sentence.Polish, sentence.EnglishTranslation, request.UserAnswer);

            var existingAnswer = await _dbContext.UserSentenceAnswers
                .FirstOrDefaultAsync(x => x.UserId == userId
                                       && x.ModuleId == request.ModuleId
                                       && x.SentenceStockId == request.SentenceStockId);

            if (existingAnswer is not null)
            {
                existingAnswer.UserAnswer = request.UserAnswer;
                existingAnswer.AiResult = sentenceCheckResult.Result;
                existingAnswer.AiExplanation = sentenceCheckResult.Explanation;
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
                    AiResult = sentenceCheckResult.Result,
                    AiExplanation = sentenceCheckResult.Explanation,
                    TeacherReviewed = false
                };
                _dbContext.UserSentenceAnswers.Add(existingAnswer);
            }

            var alreadyExists = await _dbContext.Sentences
                .AnyAsync(x => x.UserId == userId
                && x.Content.ToLower().Trim() == sentence.Polish.ToLower().Trim());

            if (!alreadyExists)
            {
                var isCorrectOrPartial = sentenceCheckResult.Result == "Correct" || sentenceCheckResult.Result == "Partial";

                _dbContext.Sentences.Add(new Sentence
                {
                    UserId = userId,
                    Content = sentence.Polish,
                    Translation = sentence.EnglishTranslation,
                    NextReviewDate = isCorrectOrPartial ? PolandTime.Today.AddDays(3) : PolandTime.Today,
                    CreatedBy = "System",
                    LastModifiedAt = PolandTime.Now,
                    LastModifiedBy = user?.Username ?? "System"
                });
            }

            await _dbContext.SaveChangesAsync();
            await TryCompleteModuleAsync(userId, request.ModuleId);
            await _dbContext.SaveChangesAsync();

            return new AnswerResultDto
            {
                Id = existingAnswer.Id,
                Polish = sentence.Polish,
                ExpectedTranslation = sentence.EnglishTranslation,
                UserAnswer = request.UserAnswer,
                AiResult = sentenceCheckResult.Result,
                AiExplanation = sentenceCheckResult.Explanation
            };
        }

        public List<AnswerResultDto> GetAnswersForModule(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;
            return MapAnswers(moduleId, userId);
        }

        public List<AnswerResultDto> GetAnswersForModuleByStudent(int moduleId, int studentId)
        {
            return MapAnswers(moduleId, studentId);
        }

        public void OverrideAnswer(int answerId, TeacherOverrideRequest request)
        {
            var answer = _dbContext.UserSentenceAnswers
                .FirstOrDefault(x => x.Id == answerId)
                ?? throw new NotFoundException("Answer not found");

            answer.TeacherOverride = request.Override;
            answer.TeacherExplanation = request.TeacherExplanation;
            answer.TeacherReviewed = true;
            _dbContext.SaveChanges();
        }

        public ModuleReportDto GenerateReport(int moduleId, int studentId)
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

        public List<CompletedSentenceModuleDto> GetCompletedSentenceModules(int studentId, DateOnly dateFrom, DateOnly dateTo)
        {
            var result = new List<CompletedSentenceModuleDto>();

            var directCompleted = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == studentId
                         && x.IsCompleted
                         && x.Module.Category == "Sentences"
                         && x.LastModifiedAt.HasValue
                         && DateOnly.FromDateTime(x.LastModifiedAt.Value.DateTime) >= dateFrom
                         && DateOnly.FromDateTime(x.LastModifiedAt.Value.DateTime) <= dateTo)
                .ToList();

            foreach (var a in directCompleted)
            {
                var totalSentences = _dbContext.ModuleSentenceSets
                    .Include(x => x.SentenceSet).ThenInclude(s => s.Items)
                    .Where(x => x.ModuleId == a.ModuleId)
                    .SelectMany(x => x.SentenceSet.Items)
                    .Count();

                var answeredCount = _dbContext.UserSentenceAnswers
                    .Count(x => x.UserId == studentId && x.ModuleId == a.ModuleId);

                result.Add(new CompletedSentenceModuleDto
                {
                    ModuleId = a.ModuleId,
                    ModuleName = a.Module.Name,
                    CompletedDate = DateOnly.FromDateTime(a.LastModifiedAt!.Value.DateTime),
                    IsFromMatrix = false,
                    TotalSentences = totalSentences,
                    AnsweredCount = answeredCount
                });
            }

            var matrixCompleted = _dbContext.UserMatrixModuleCompletions
                .Include(x => x.MatrixModule)
                    .ThenInclude(mm => mm.Module)
                .Include(x => x.MatrixModule)
                    .ThenInclude(mm => mm.Matrix)
                .Where(x => x.UserId == studentId
                         && x.MatrixModule.Module.Category == "Sentences"
                         && x.CompletedDate >= dateFrom
                         && x.CompletedDate <= dateTo)
                .ToList();

            foreach (var c in matrixCompleted)
            {
                var moduleId = c.MatrixModule.ModuleId;

                var totalSentences = _dbContext.ModuleSentenceSets
                    .Include(x => x.SentenceSet).ThenInclude(s => s.Items)
                    .Where(x => x.ModuleId == moduleId)
                    .SelectMany(x => x.SentenceSet.Items)
                    .Count();

                var answeredCount = _dbContext.UserSentenceAnswers
                    .Count(x => x.UserId == studentId && x.ModuleId == moduleId);

                result.Add(new CompletedSentenceModuleDto
                {
                    ModuleId = moduleId,
                    ModuleName = c.MatrixModule.Module.Name,
                    CompletedDate = c.CompletedDate,
                    IsFromMatrix = true,
                    MatrixName = c.MatrixModule.Matrix.Name,
                    TotalSentences = totalSentences,
                    AnsweredCount = answeredCount
                });
            }

            return result
                .OrderBy(x => x.CompletedDate)
                .ToList();
        }

        public DateRangeReportDto GenerateDateRangeReport(int studentId, DateOnly dateFrom, DateOnly dateTo)
        {
            var student = _dbContext.Users.FirstOrDefault(x => x.Id == studentId)
                ?? throw new NotFoundException("Student not found");

            var modules = GetCompletedSentenceModules(studentId, dateFrom, dateTo);

            var moduleReports = modules
                .Select(m => GenerateReport(m.ModuleId, studentId))
                .ToList();

            return new DateRangeReportDto
            {
                StudentUsername = student.Username,
                DateFrom = dateFrom,
                DateTo = dateTo,
                GeneratedDate = PolandTime.Today,
                Modules = moduleReports,
                TotalCorrect = moduleReports.Sum(m => m.CorrectCount),
                TotalPartial = moduleReports.Sum(m => m.PartialCount),
                TotalIncorrect = moduleReports.Sum(m => m.IncorrectCount),
                TotalSentences = moduleReports.Sum(m => m.TotalSentences)
            };
        }



        private List<AnswerResultDto> MapAnswers(int moduleId, int userId)
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

        private async Task TryCompleteModuleAsync(int userId, int moduleId)
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
                await AssignModuleCompletionPointsAsync(userId, moduleId, directAssignment);
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
                        await AssignModuleCompletionPointsAsync(userId, moduleId, fakeAssignment);
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

        private async Task AssignModuleCompletionPointsAsync(int userId, int moduleId, UserModuleAssignment? assignment)
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

            _lessonPanelService.AddActivityPoints(userId, totalPoints, reason);
        }

        public List<inzBackend.Models.SentenceModels.SentenceModuleLiveDto> GetSentenceModulesForLiveRoom(int? studentId = null)
        {
            var query = _dbContext.UserSentenceAnswers
                .Include(x => x.User).ThenInclude(u => u.Profile)
                .Include(x => x.Module)
                .Include(x => x.SentenceStock)
                .AsQueryable();

            if (studentId.HasValue)
            {
                query = query.Where(x => x.UserId == studentId.Value);
            }
            else
            {
                var currentUserId = _userContextService.GetUserId!.Value;
                var role = _userContextService.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (role != "Admin")
                {
                    query = query.Where(x => x.UserId == currentUserId);
                }
            }

            var grouped = query
                .ToList()
                .GroupBy(x => new { x.ModuleId, x.UserId })
                .Select(g =>
                {
                    var first = g.First();
                    var totalInModule = _dbContext.ModuleSentenceSets
                        .Where(ms => ms.ModuleId == g.Key.ModuleId)
                        .Join(_dbContext.SentenceSetItems, ms => ms.SentenceSetId, ssi => ssi.SentenceSetId, (ms, ssi) => ssi)
                        .Select(ssi => ssi.SentenceStockId)
                        .Distinct()
                        .Count();

                    if (totalInModule == 0) totalInModule = g.Count();

                    int correct = g.Count(x => (x.TeacherOverride ?? x.AiResult) == "Correct");
                    int partial = g.Count(x => (x.TeacherOverride ?? x.AiResult) == "Partial");
                    int incorrect = g.Count(x => (x.TeacherOverride ?? x.AiResult) == "Incorrect");
                    bool allReviewed = g.All(x => x.TeacherReviewed);

                    var maxDate = g.Max(x => x.LastModifiedAt);

                    return new inzBackend.Models.SentenceModels.SentenceModuleLiveDto
                    {
                        ModuleId = g.Key.ModuleId,
                        ModuleName = first.Module?.Name ?? "Module",
                        StudentId = g.Key.UserId,
                        StudentUsername = first.User?.Username ?? "Student",
                        StudentAvatarUrl = first.User?.Profile?.AvatarUrl,
                        TotalSentences = totalInModule,
                        AnsweredSentences = g.Count(),
                        CorrectCount = correct,
                        PartialCount = partial,
                        IncorrectCount = incorrect,
                        IsReviewed = allReviewed,
                        LastAnswerDate = maxDate.HasValue ? maxDate.Value.DateTime : null
                    };
                })
                .OrderByDescending(x => x.LastAnswerDate)
                .ToList();

            return grouped;
        }

        public List<inzBackend.Models.SentenceModels.SentenceAnswerLiveDto> GetSentenceAnswersForLiveRoom(int moduleId, int? studentId = null)
        {
            var targetUserId = studentId ?? _userContextService.GetUserId!.Value;
            var role = _userContextService.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role != "Admin" && !studentId.HasValue)
            {
                targetUserId = _userContextService.GetUserId!.Value;
            }

            var setIds = _dbContext.ModuleSentenceSets
                .Where(x => x.ModuleId == moduleId)
                .Select(x => x.SentenceSetId)
                .ToList();

            var sentenceItems = _dbContext.SentenceSetItems
                .Include(x => x.SentenceStock)
                .Where(x => setIds.Contains(x.SentenceSetId))
                .OrderBy(x => x.SentenceSetId).ThenBy(x => x.Order)
                .ToList();

            var userAnswers = _dbContext.UserSentenceAnswers
                .Include(x => x.User).ThenInclude(u => u.Profile)
                .Include(x => x.Module)
                .Where(x => x.ModuleId == moduleId && x.UserId == targetUserId)
                .ToDictionary(x => x.SentenceStockId);

            var result = new List<inzBackend.Models.SentenceModels.SentenceAnswerLiveDto>();

            foreach (var item in sentenceItems)
            {
                userAnswers.TryGetValue(item.SentenceStockId, out var ans);

                if (ans != null)
                {
                    result.Add(new inzBackend.Models.SentenceModels.SentenceAnswerLiveDto
                    {
                        Id = ans.Id,
                        ModuleId = moduleId,
                        ModuleName = ans.Module?.Name ?? "Module",
                        SentenceStockId = item.SentenceStockId,
                        Polish = item.SentenceStock.Polish,
                        ExpectedTranslation = item.SentenceStock.EnglishTranslation,
                        UserAnswer = ans.UserAnswer,
                        AdminCorrection = ans.AdminCorrection ?? ans.UserAnswer,
                        AiResult = ans.AiResult,
                        AiExplanation = ans.AiExplanation,
                        TeacherOverride = ans.TeacherOverride,
                        TeacherExplanation = ans.TeacherExplanation,
                        TeacherReviewed = ans.TeacherReviewed,
                        StudentUsername = ans.User?.Username ?? "",
                        StudentAvatarUrl = ans.User?.Profile?.AvatarUrl
                    });
                }
            }

            return result;
        }

        public inzBackend.Models.SentenceModels.SentenceAnswerLiveDto GetSentenceAnswerDetailForLiveRoom(int answerId)
        {
            var ans = _dbContext.UserSentenceAnswers
                .Include(x => x.User).ThenInclude(u => u.Profile)
                .Include(x => x.Module)
                .Include(x => x.SentenceStock)
                .FirstOrDefault(x => x.Id == answerId)
                ?? throw new NotFoundException($"Sentence answer {answerId} not found");

            return new inzBackend.Models.SentenceModels.SentenceAnswerLiveDto
            {
                Id = ans.Id,
                ModuleId = ans.ModuleId,
                ModuleName = ans.Module?.Name ?? "Module",
                SentenceStockId = ans.SentenceStockId,
                Polish = ans.SentenceStock.Polish,
                ExpectedTranslation = ans.SentenceStock.EnglishTranslation,
                UserAnswer = ans.UserAnswer,
                AdminCorrection = ans.AdminCorrection ?? ans.UserAnswer,
                AiResult = ans.AiResult,
                AiExplanation = ans.AiExplanation,
                TeacherOverride = ans.TeacherOverride,
                TeacherExplanation = ans.TeacherExplanation,
                TeacherReviewed = ans.TeacherReviewed,
                StudentUsername = ans.User?.Username ?? "",
                StudentAvatarUrl = ans.User?.Profile?.AvatarUrl
            };
        }

        public inzBackend.Models.SentenceModels.SentenceAnswerLiveDto SaveSentenceReview(int answerId, inzBackend.Models.SentenceModels.SaveSentenceReviewRequest request)
        {
            var ans = _dbContext.UserSentenceAnswers
                .Include(x => x.User).ThenInclude(u => u.Profile)
                .Include(x => x.Module)
                .Include(x => x.SentenceStock)
                .FirstOrDefault(x => x.Id == answerId)
                ?? throw new NotFoundException($"Sentence answer {answerId} not found");

            if (request.AdminCorrection != null)
            {
                ans.AdminCorrection = request.AdminCorrection;
            }
            if (request.TeacherOverride != null)
            {
                ans.TeacherOverride = request.TeacherOverride;
            }
            if (request.TeacherExplanation != null)
            {
                ans.TeacherExplanation = request.TeacherExplanation;
            }
            ans.TeacherReviewed = true;

            _dbContext.SaveChanges();

            return new inzBackend.Models.SentenceModels.SentenceAnswerLiveDto
            {
                Id = ans.Id,
                ModuleId = ans.ModuleId,
                ModuleName = ans.Module?.Name ?? "Module",
                SentenceStockId = ans.SentenceStockId,
                Polish = ans.SentenceStock.Polish,
                ExpectedTranslation = ans.SentenceStock.EnglishTranslation,
                UserAnswer = ans.UserAnswer,
                AdminCorrection = ans.AdminCorrection ?? ans.UserAnswer,
                AiResult = ans.AiResult,
                AiExplanation = ans.AiExplanation,
                TeacherOverride = ans.TeacherOverride,
                TeacherExplanation = ans.TeacherExplanation,
                TeacherReviewed = ans.TeacherReviewed,
                StudentUsername = ans.User?.Username ?? "",
                StudentAvatarUrl = ans.User?.Profile?.AvatarUrl
            };
        }

        public List<inzBackend.Models.SentenceModels.SentenceAnswerCommentDto> GetCommentsForSentenceAnswer(int answerId)
        {
            return _dbContext.UserSentenceAnswerComments
                .Where(x => x.UserSentenceAnswerId == answerId && !x.IsArchived)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => new inzBackend.Models.SentenceModels.SentenceAnswerCommentDto
                {
                    Id = x.Id,
                    UserSentenceAnswerId = x.UserSentenceAnswerId,
                    SelectedText = x.SelectedText,
                    NoteContent = x.NoteContent,
                    Category = x.Category,
                    Author = x.Author,
                    Timestamp = x.Timestamp,
                    IsArchived = x.IsArchived
                })
                .ToList();
        }

        public inzBackend.Models.SentenceModels.SentenceAnswerCommentDto AddCommentToSentenceAnswer(int answerId, inzBackend.Models.SentenceModels.CreateSentenceAnswerCommentRequest request)
        {
            var ans = _dbContext.UserSentenceAnswers
                .FirstOrDefault(x => x.Id == answerId)
                ?? throw new NotFoundException($"Sentence answer {answerId} not found");

            var authorName = _userContextService.GetUserName ?? "Teacher";

            var comment = new inzBackend.Entities.Assignments.UserSentenceAnswerComment
            {
                UserSentenceAnswerId = answerId,
                SelectedText = request.SelectedText,
                NoteContent = request.NoteContent,
                Category = request.Category,
                Author = authorName,
                Timestamp = PolandTime.DateTimeNow,
                IsArchived = false
            };

            _dbContext.UserSentenceAnswerComments.Add(comment);
            _dbContext.SaveChanges();

            return new inzBackend.Models.SentenceModels.SentenceAnswerCommentDto
            {
                Id = comment.Id,
                UserSentenceAnswerId = comment.UserSentenceAnswerId,
                SelectedText = comment.SelectedText,
                NoteContent = comment.NoteContent,
                Category = comment.Category,
                Author = comment.Author,
                Timestamp = comment.Timestamp,
                IsArchived = comment.IsArchived
            };
        }

        public void ArchiveSentenceAnswerComment(int commentId)
        {
            var comment = _dbContext.UserSentenceAnswerComments
                .FirstOrDefault(x => x.Id == commentId)
                ?? throw new NotFoundException($"Comment {commentId} not found");

            comment.IsArchived = true;
            _dbContext.SaveChanges();
        }
    }
}