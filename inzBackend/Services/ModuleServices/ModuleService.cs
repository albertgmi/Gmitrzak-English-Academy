using AutoMapper;
using inzBackend.Entities;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.Curriculum;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Models.StudentCourseModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.ModuleServices
{
    public class ModuleService : IModuleService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        
        public ModuleService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        public List<ModuleDto> GetAllModules()
        {
            var modules = _dbContext.Modules
                .Include(m => m.MatrixModules).ThenInclude(mm => mm.Matrix)
                .Include(m => m.TheaterItem)
                .ToList();

            return _mapper.Map<List<ModuleDto>>(modules);
        }

        public List<ModuleDto> GetSentenceModulesForStudent(int studentId)
        {
            var studentMatrixIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == studentId)
                .Select(x => x.MatrixId)
                .ToList();

            var studentDirectModuleIds = _dbContext.UserModuleAssignments
                .Where(x => x.UserId == studentId)
                .Select(x => x.ModuleId)
                .ToList();

            var modules = _dbContext.Modules
                .Where(m => m.Category == "Sentences" &&
                            (studentDirectModuleIds.Contains(m.Id) ||
                             m.MatrixModules.Any(mm => studentMatrixIds.Contains(mm.MatrixId))))
                .ToList();

            return _mapper.Map<List<ModuleDto>>(modules);
        }

        public ModuleDto CreateModule(CreateModuleRequest request)
        {
            var module = new Module
            {
                Name = request.Name,
                Description = request.Description,
                IsHidden = request.IsHidden ?? false,
                Category = request.Category,
                TheaterItemId = request.Category == "Watching" ? request.TheaterItemId : null
            };

            _dbContext.Modules.Add(module);
            _dbContext.SaveChanges();

            if (request.Category == "Presentation" &&
                (!string.IsNullOrWhiteSpace(request.PresentationUrl) ||
                 !string.IsNullOrWhiteSpace(request.PresentationText)))
            {
                _dbContext.ModulePresentations.Add(new ModulePresentation
                {
                    ModuleId = module.Id,
                    Url = request.PresentationUrl,
                    Text = request.PresentationText
                });
            }

            if (request.Category == "Essay")
                module.EssayPrompt = request.EssayPrompt;

            _dbContext.SaveChanges();
            return _mapper.Map<ModuleDto>(module);
        }

        public void DeleteModule(int moduleId)
        {
            var module = _dbContext.Modules.FirstOrDefault(x => x.Id == moduleId)
                ?? throw new NotFoundException($"Module with id {moduleId} was not found");

            _dbContext.Modules.Remove(module);
            _dbContext.SaveChanges();
        }

        public void UpdateModule(int moduleId, UpdateModuleRequest request)
        {
            var module = _dbContext.Modules
                .Include(m => m.Presentation)
                .FirstOrDefault(x => x.Id == moduleId)
                ?? throw new NotFoundException("Module not found");

            if (request.Name is not null) module.Name = request.Name;
            if (request.Description is not null) module.Description = request.Description;
            if (request.IsHidden is not null) module.IsHidden = request.IsHidden;
            if (request.Category is not null) module.Category = request.Category;

            module.TheaterItemId = request.Category == "Watching" ? request.TheaterItemId : null;

            if (request.Category == "Presentation")
            {
                if (module.Presentation is not null)
                {
                    module.Presentation.Url = request.PresentationUrl;
                    module.Presentation.Text = request.PresentationText;
                }
                else if (!string.IsNullOrWhiteSpace(request.PresentationUrl) ||
                         !string.IsNullOrWhiteSpace(request.PresentationText))
                {
                    _dbContext.ModulePresentations.Add(new ModulePresentation
                    {
                        ModuleId = moduleId,
                        Url = request.PresentationUrl,
                        Text = request.PresentationText
                    });
                }
            }
            else if (module.Presentation is not null)
            {
                _dbContext.ModulePresentations.Remove(module.Presentation);
            }

            if (request.Category == "Essay")
                module.EssayPrompt = request.EssayPrompt;

            _dbContext.SaveChanges();
        }

        public void AssignMatrix(int moduleId, int matrixId, AssignModuleToMatrixRequest request)
        {
            var module = _dbContext.Modules.FirstOrDefault(x => x.Id == moduleId)
                ?? throw new NotFoundException($"Module with id {moduleId} was not found");

            var matrix = _dbContext.Matrices.FirstOrDefault(x => x.Id == matrixId)
                ?? throw new NotFoundException($"Matrix with id {matrixId} was not found");

            var repeatWeeks = request.RepeatWeeks is > 1 ? request.RepeatWeeks.Value : 1;
            var targetWeeks = Enumerable.Range(request.WeekNumber, repeatWeeks).ToList();

            var conflicting = _dbContext.MatrixModules
                .Where(mm => mm.MatrixId == matrixId
                          && mm.ModuleId == moduleId
                          && targetWeeks.Contains(mm.WeekNumber))
                .Select(mm => mm.WeekNumber)
                .ToList();

            if (conflicting.Any())
                throw new BadRequestException(
                    $"Module {module.Name} is already assigned to matrix {matrix.Name} for week(s): {string.Join(", ", conflicting)}");

            foreach (var week in targetWeeks)
            {
                _dbContext.MatrixModules.Add(new MatrixModule
                {
                    MatrixId = matrixId,
                    ModuleId = moduleId,
                    WeekNumber = week,
                    DayOfWeek = request.DayOfWeek
                });
            }

            _dbContext.SaveChanges();
        }

        public void DetachMatrix(int moduleId, int matrixId)
        {
            var module = _dbContext.Modules.FirstOrDefault(x => x.Id == moduleId)
                ?? throw new NotFoundException($"Module with id {moduleId} was not found");

            var matrix = _dbContext.Matrices.FirstOrDefault(x => x.Id == matrixId)
                ?? throw new NotFoundException($"Matrix with id {matrixId} was not found");

            var matrixModule = _dbContext.MatrixModules
                .FirstOrDefault(mm => mm.MatrixId == matrixId && mm.ModuleId == moduleId)
                ?? throw new NotFoundException(
                    $"Module {module.Name} is not assigned to matrix {matrix.Name}");

            _dbContext.MatrixModules.Remove(matrixModule);
            _dbContext.SaveChanges();
        }
        public StudentModuleDto? GetStudentModule(int userId, int moduleId)
        {
            var today = PolandTime.Today;

            var direct = _dbContext.UserModuleAssignments
                .Include(x => x.Module).ThenInclude(m => m.Presentation)
                .Include(x => x.Module).ThenInclude(m => m.TheaterItem)
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            if (direct is not null)
            {
                var assignedDate = DateOnly.FromDateTime(direct.CreatedAt.DateTime);
                var activityStatus1 = GetActivityStatus(userId, direct.Module.Category, today, assignedDate);

                return new StudentModuleDto
                {
                    Id = direct.Id,
                    ModuleId = moduleId,
                    Name = direct.Module.Name,
                    Description = direct.Module.Description ?? string.Empty,
                    Category = direct.Module.Category,
                    UnlockDate = direct.DueDate,
                    IsUnlocked = true,
                    IsCompleted = direct.IsCompleted,
                    IsOverdue = direct.DueDate < today,
                    Url = direct.Module.TheaterItem?.Url,
                    ActivityDaysCount = activityStatus1.Streak,
                    ActivityDaysRequired = activityStatus1.Required,
                    CanComplete = activityStatus1.CanComplete,
                    CompletionBlockReason = activityStatus1.BlockReason,
                    PresentationUrl = direct.Module.Presentation?.Url,
                    PresentationText = direct.Module.Presentation?.Text
                };
            }

            var userMatrixIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixId)
                .ToList();

            if (!userMatrixIds.Any()) return null;

            var instances = GetMatrixModuleInstances(userId, moduleId, userMatrixIds);
            if (!instances.Any()) return null;

            var selected = SelectCurrentInstance(instances, today);
            if (selected is null) return null;

            var (mm, unlockDate, isCompleted) = selected.Value;

            var activityStatus = GetActivityStatus(userId, mm.Module.Category, today, unlockDate);

            return new StudentModuleDto
            {
                Id = mm.Id,
                ModuleId = moduleId,
                Name = mm.Module.Name,
                Description = mm.Module.Description ?? string.Empty,
                Category = mm.Module.Category,
                UnlockDate = unlockDate,
                IsUnlocked = today >= unlockDate,
                IsCompleted = isCompleted,
                IsOverdue = false,
                Url = mm.Module.TheaterItem?.Url,
                ActivityDaysCount = activityStatus.Streak,
                ActivityDaysRequired = activityStatus.Required,
                CanComplete = activityStatus.CanComplete,
                CompletionBlockReason = activityStatus.BlockReason,
                PresentationUrl = mm.Module.Presentation?.Url,
                PresentationText = mm.Module.Presentation?.Text
            };
        }

        public void CompleteStudentModule(int userId, int moduleId)
        {
            var today = PolandTime.Today;

            var direct = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            if (direct is not null)
            {
                if (direct.IsCompleted) return;

                var assignedDate = DateOnly.FromDateTime(direct.CreatedAt.DateTime);
                var activityStatus1 = GetActivityStatus(userId, direct.Module.Category, today, assignedDate);

                if (!activityStatus1.CanComplete)
                    throw new BadRequestException(activityStatus1.BlockReason ?? "Not enough consecutive days.");

                direct.IsCompleted = true;
                _dbContext.SaveChanges();
                return;
            }

            var userMatrixIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixId)
                .ToList();

            var instances = GetMatrixModuleInstances(userId, moduleId, userMatrixIds);
            if (!instances.Any())
                throw new NotFoundException("Module assignment not found");

            var current = instances
                .Where(x => !x.isCompleted)
                .OrderBy(x => x.unlockDate)
                .FirstOrDefault();

            if (current.mm is null)
                return;

            var (mm, unlockDate, _) = current;

            var activityStatus = GetActivityStatus(userId, mm.Module.Category, today, unlockDate);

            if (!activityStatus.CanComplete)
                throw new BadRequestException(activityStatus.BlockReason ?? "Not enough consecutive days.");

            _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
            {
                UserId = userId,
                MatrixModuleId = mm.Id,
                CompletedDate = today
            });

            _dbContext.SaveChanges();
        }

        private List<(MatrixModule mm, DateOnly unlockDate, bool isCompleted)> GetMatrixModuleInstances(int userId, int moduleId,
            List<int> userMatrixIds)
        {
            var instances = _dbContext.MatrixModules
                .Include(x => x.Module).ThenInclude(m => m.Presentation)
                .Include(x => x.Module).ThenInclude(m => m.TheaterItem)
                .Include(x => x.Matrix)
                .Where(x => x.ModuleId == moduleId && userMatrixIds.Contains(x.MatrixId))
                .ToList();

            var result = new List<(MatrixModule, DateOnly, bool)>();

            foreach (var mm in instances)
            {
                var ma = _dbContext.UserMatrixAssignments
                    .FirstOrDefault(x => x.UserId == userId && x.MatrixId == mm.MatrixId);
                if (ma is null) continue;

                var unlockDate = ma.StartDate
                    .AddDays((mm.WeekNumber - 1) * mm.Matrix.RefreshIntervalDays)
                    .AddDays(mm.DayOfWeek - 1);

                var isCompleted = _dbContext.UserMatrixModuleCompletions
                    .Any(x => x.UserId == userId && x.MatrixModuleId == mm.Id);

                result.Add((mm, unlockDate, isCompleted));
            }

            return result;
        }

        private static (MatrixModule mm, DateOnly unlockDate, bool isCompleted)? SelectCurrentInstance(
            List<(MatrixModule mm, DateOnly unlockDate, bool isCompleted)> instances, DateOnly today)
        {
            if (!instances.Any()) return null;

            var unlockedIncomplete = instances
                .Where(x => today >= x.unlockDate && !x.isCompleted)
                .OrderBy(x => x.unlockDate)
                .FirstOrDefault();
            if (unlockedIncomplete.mm is not null) return unlockedIncomplete;

            var unlockedCompleted = instances
                .Where(x => today >= x.unlockDate && x.isCompleted)
                .OrderByDescending(x => x.unlockDate)
                .FirstOrDefault();
            if (unlockedCompleted.mm is not null) return unlockedCompleted;

            return instances.OrderBy(x => x.unlockDate).First();
        }

        private ActivityStatus GetActivityStatus(int userId, string category, DateOnly today, DateOnly? assignedDate = null)
        {
            const int REQUIRED = 3;
            var countFrom = assignedDate ?? DateOnly.MinValue;

            switch (category)
            {
                case "Flashcards":
                    {
                        var dates = _dbContext.FlashcardStudyLogs
                            .Where(x => x.UserId == userId && x.StudyDate >= countFrom)
                            .Select(x => x.StudyDate)
                            .Distinct()
                            .OrderByDescending(x => x)
                            .ToList();

                        var streak = CountConsecutiveStreak(dates, today);
                        return new ActivityStatus
                        {
                            Streak = streak,
                            Required = REQUIRED,
                            CanComplete = streak >= REQUIRED,
                            BlockReason = streak >= REQUIRED ? null : $"Study flashcards for {REQUIRED - streak} more consecutive day(s)."
                        };
                    }

                case "SentenceFlashcards":
                    {
                        var dates = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId
                                     && x.Section == "sentenceflashcards"
                                     && x.ActivityDate >= countFrom)
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .OrderByDescending(x => x)
                            .ToList();

                        var streak = CountConsecutiveStreak(dates, today);
                        return new ActivityStatus
                        {
                            Streak = streak,
                            Required = REQUIRED,
                            CanComplete = streak >= REQUIRED,
                            BlockReason = streak >= REQUIRED ? null : $"Practice sentence flashcards for {REQUIRED - streak} more consecutive day(s)."
                        };
                    }

                case "Memories":
                    {
                        var dates = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId
                                     && x.Section == "memories"
                                     && x.ActivityDate >= countFrom)
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .OrderByDescending(x => x)
                            .ToList();

                        var streak = CountConsecutiveStreak(dates, today);
                        return new ActivityStatus
                        {
                            Streak = streak,
                            Required = REQUIRED,
                            CanComplete = streak >= REQUIRED,
                            BlockReason = streak >= REQUIRED ? null : $"Visit Memories for {REQUIRED - streak} more consecutive day(s)."
                        };
                    }

                case "Pronunciation":
                    {
                        var dates = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId
                                     && x.Section == "pronunciation"
                                     && x.ActivityDate >= countFrom)
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .OrderByDescending(x => x)
                            .ToList();

                        var streak = CountConsecutiveStreak(dates, today);
                        return new ActivityStatus
                        {
                            Streak = streak,
                            Required = REQUIRED,
                            CanComplete = streak >= REQUIRED,
                            BlockReason = streak >= REQUIRED ? null : $"Practice pronunciation for {REQUIRED - streak} more consecutive day(s)."
                        };
                    }

                case "Sentences":
                case "Presentation":
                case "General":
                default:
                    return new ActivityStatus
                    {
                        Streak = 0,
                        Required = 0,
                        CanComplete = true,
                        BlockReason = null
                    };
            }
        }

        private static int CountConsecutiveStreak(List<DateOnly> datesDesc, DateOnly today)
        {
            if (!datesDesc.Any()) return 0;

            var mostRecent = datesDesc.First();
            if (mostRecent < today.AddDays(-1)) return 0;

            var streak = 0;
            var expected = mostRecent;

            foreach (var date in datesDesc)
            {
                if (date == expected)
                {
                    streak++;
                    expected = expected.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }
    }
}