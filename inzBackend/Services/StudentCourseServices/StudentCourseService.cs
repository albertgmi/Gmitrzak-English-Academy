using inzBackend.Entities;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.Curriculum;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Models.StudentCourseModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentCourseServices
{
    public class StudentCourseService : IStudentCourseService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly ILessonPanelService _lessonPanelService;

        public StudentCourseService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            ILessonPanelService lessonPanelService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _lessonPanelService = lessonPanelService;
        }

        public List<StudentAssignmentDto> GetStudentsAssignments()
        {
            var userId = _userContextService.GetUserId;
            var today = PolandTime.Today;
            var currentWeekMonday = WeekHelper.GetWeekMonday(today);

            var assignments = _dbContext.UserMatrixAssignments
                .Include(uma => uma.Matrix)
                    .ThenInclude(m => m.MatrixModules)
                        .ThenInclude(mm => mm.Module)
                .Where(uma => uma.UserId == userId)
                .ToList();

            var completedMatrixModuleIds = _dbContext.UserMatrixModuleCompletions
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixModuleId)
                .ToList();

            var dueDateOverrides = _dbContext.UserMatrixModuleDueDateOverrides
                .Where(x => x.UserId == userId)
                .ToDictionary(x => x.MatrixModuleId, x => x.NewDeadline);

            return assignments
                .Select(a => MapToStudentAssignmentDto(
                    a, completedMatrixModuleIds, dueDateOverrides, today, currentWeekMonday))
                .ToList();
        }

        public void CompleteModule(int matrixModuleId)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var alreadyCompleted = _dbContext.UserMatrixModuleCompletions
                .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);
            if (alreadyCompleted)
                throw new BadRequestException("Matrix module is already completed");

            var matrixModule = _dbContext.MatrixModules
                .Include(x => x.Module)
                .Include(x => x.Matrix)
                .FirstOrDefault(x => x.Id == matrixModuleId)
                ?? throw new NotFoundException("Matrix module not found");

            var matrixAssignment = _dbContext.UserMatrixAssignments
                .FirstOrDefault(x => x.UserId == userId && x.MatrixId == matrixModule.MatrixId);

            var deadlineOverride = GetDueDateOverride(userId, matrixModuleId);

            var deadline = matrixAssignment is not null
                ? MatrixModuleDateHelper.GetEffectiveDeadline(
                    matrixAssignment.StartDate, matrixModule.WeekNumber, matrixModule.DayOfWeek,
                    matrixAssignment.Matrix.RefreshIntervalDays, deadlineOverride)
                : today;

            var activityStatus = GetActivityStatus(userId, matrixModule.ModuleId,
                matrixModule.Module.Category, today, deadline);

            if (!activityStatus.CanComplete)
                throw new BadRequestException(activityStatus.BlockReason ?? "Not enough activity days.");

            _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
            {
                UserId = userId,
                MatrixModuleId = matrixModuleId,
                CompletedDate = today
            });

            _lessonPanelService.AddActivityPoints(
                userId, 10,
                $"Completed curriculum module ({matrixModule.Module.Name})");

            _dbContext.SaveChanges();
        }

        public void UncompleteModule(int matrixModuleId)
        {
            var userId = _userContextService.GetUserId;

            var matrixModule = _dbContext.MatrixModules
                .Include(x => x.Module)
                .Include(x => x.Matrix)
                .FirstOrDefault(x => x.Id == matrixModuleId)
                ?? throw new NotFoundException("Matrix module not found");

            var completion = _dbContext.UserMatrixModuleCompletions
                .FirstOrDefault(x => x.UserId == userId
                                  && x.MatrixModuleId == matrixModuleId)
                ?? throw new BadRequestException("Matrix module is not completed");

            _lessonPanelService.AddActivityPoints(
                userId!.Value, -10,
                $"Reversed completion of curriculum module ({matrixModule.Module.Name})");

            _dbContext.UserMatrixModuleCompletions.Remove(completion);
            _dbContext.SaveChanges();
        }

        public List<StudentModuleDto> GetSingleModules()
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var assignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module).ThenInclude(m => m.Presentation)
                .Include(x => x.Module).ThenInclude(x => x.TheaterItem)
                .Where(x => x.UserId == userId && !x.IsCompleted)
                .ToList();

            return assignments.Select((x, index) => BuildModuleDto(
                id: x.Id,
                moduleId: x.ModuleId,
                module: x.Module,
                order: index + 1,
                weekNumber: 0,
                dayOfWeek: 0,
                deadline: x.DueDate,
                unlockDate: DateOnly.FromDateTime(x.CreatedAt.DateTime),
                isUnlocked: true,
                isCompleted: x.IsCompleted,
                isOverdue: x.DueDate < today,
                userId: userId,
                today: today,
                url: x.Module?.TheaterItem?.Url,
                assignedDate: DateOnly.FromDateTime(x.CreatedAt.DateTime)
            )).ToList();
        }

        public void CompleteSingleModule(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var assignment = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .FirstOrDefault(x => x.Id == id && x.UserId == userId)
                ?? throw new NotFoundException("Module assignment not found");

            var assignedDate = DateOnly.FromDateTime(assignment.CreatedAt.DateTime);

            var activityStatus = GetActivityStatus(userId, assignment.ModuleId,
                assignment.Module.Category, today, assignedDate);

            if (!activityStatus.CanComplete)
                throw new BadRequestException(activityStatus.BlockReason ?? "Not enough activity days.");

            assignment.IsCompleted = true;
            _lessonPanelService.AddActivityPoints(
                userId, 10, $"Completed assignment {assignment.Module.Name}");
            _dbContext.SaveChanges();
        }

        public void UncompleteSingleModule(int id)
        {
            var userId = _userContextService.GetUserId;

            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id && x.UserId == userId)
                ?? throw new NotFoundException("Module assignment not found");

            assignment.IsCompleted = false;
            _lessonPanelService.AddActivityPoints(
                userId!.Value, -10,
                $"Reversed completion of additional assignment ({assignment.Module.Name})");
            _dbContext.SaveChanges();
        }

        public List<StudentModuleDto> GetCompletedSingleModules()
        {
            var userId = _userContextService.GetUserId;

            return _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && x.IsCompleted)
                .Select(x => new StudentModuleDto
                {
                    Id = x.Id,
                    ModuleId = x.ModuleId,
                    Name = x.Module.Name,
                    Description = x.Module.Description,
                    Category = x.Module.Category,
                    Order = 0,
                    WeekNumber = 0,
                    DayOfWeek = 0,
                    UnlockDate = x.DueDate,
                    IsUnlocked = true,
                    IsCompleted = true
                })
                .ToList();
        }

        public StudentModuleDto? GetStudentModule(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var directAssignment = _dbContext.UserModuleAssignments
                .Include(x => x.Module).ThenInclude(m => m.Presentation)
                .Include(x => x.Module).ThenInclude(m => m.TheaterItem)
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            if (directAssignment is not null)
            {
                return BuildModuleDto(
                    id: directAssignment.Id,
                    moduleId: moduleId,
                    module: directAssignment.Module,
                    order: 1,
                    weekNumber: 0,
                    dayOfWeek: 0,
                    deadline: directAssignment.DueDate,
                    unlockDate: DateOnly.FromDateTime(directAssignment.CreatedAt.DateTime),
                    isUnlocked: true,
                    isCompleted: directAssignment.IsCompleted,
                    isOverdue: directAssignment.DueDate < today,
                    userId: userId,
                    today: today,
                    url: directAssignment.Module?.TheaterItem?.Url,
                    assignedDate: DateOnly.FromDateTime(directAssignment.CreatedAt.DateTime)
                );
            }

            var userMatrixIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixId)
                .ToList();

            var matrixModule = _dbContext.MatrixModules
                .Include(x => x.Module).ThenInclude(m => m.Presentation)
                .Include(x => x.Module).ThenInclude(m => m.TheaterItem)
                .Include(x => x.Matrix)
                .FirstOrDefault(x => x.ModuleId == moduleId
                                  && userMatrixIds.Contains(x.MatrixId));

            if (matrixModule is null) return null;

            var matrixAssignment = _dbContext.UserMatrixAssignments
                .FirstOrDefault(x => x.UserId == userId && x.MatrixId == matrixModule.MatrixId);

            if (matrixAssignment is null) return null;

            var originalDeadline = MatrixModuleDateHelper.ComputeDeadline(
                matrixAssignment.StartDate, matrixModule.WeekNumber, matrixModule.DayOfWeek,
                matrixAssignment.Matrix.RefreshIntervalDays);

            var deadlineOverride = GetDueDateOverride(userId, matrixModule.Id);
            var effectiveDeadline = deadlineOverride ?? originalDeadline;

            var unlockDate = WeekHelper.GetWeekMonday(originalDeadline);
            var isCompleted = _dbContext.UserMatrixModuleCompletions
                .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModule.Id);

            return BuildModuleDto(
                id: matrixModule.Id,
                moduleId: moduleId,
                module: matrixModule.Module,
                order: 1,
                weekNumber: matrixModule.WeekNumber,
                dayOfWeek: matrixModule.DayOfWeek,
                unlockDate: unlockDate,
                deadline: effectiveDeadline,
                isUnlocked: today >= unlockDate,
                isCompleted: isCompleted,
                isOverdue: today > effectiveDeadline && !isCompleted,
                userId: userId,
                today: today,
                url: matrixModule.Module?.TheaterItem?.Url,
                assignedDate: unlockDate
            );
        }

        public void CompleteStudentModule(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var direct = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            if (direct is not null)
            {
                if (direct.IsCompleted) return;

                var assignedDate = DateOnly.FromDateTime(direct.CreatedAt.DateTime);
                var activityStatus = GetActivityStatus(userId, moduleId, direct.Module.Category, today, assignedDate);
                if (!activityStatus.CanComplete)
                    throw new BadRequestException(activityStatus.BlockReason ?? "Not enough activity days.");

                direct.IsCompleted = true;
                _lessonPanelService.AddActivityPoints(
                    userId, 10, $"Completed assignment {direct.Module.Name}");
                _dbContext.SaveChanges();
                return;
            }

            var userMatrixIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixId)
                .ToList();

            var matrixModule = _dbContext.MatrixModules
                .Include(x => x.Module)
                .Include(x => x.Matrix)
                .FirstOrDefault(x => x.ModuleId == moduleId && userMatrixIds.Contains(x.MatrixId))
                ?? throw new NotFoundException("Module assignment not found");

            var alreadyCompleted = _dbContext.UserMatrixModuleCompletions
                .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModule.Id);

            if (alreadyCompleted) return;

            var ma = _dbContext.UserMatrixAssignments
                .First(x => x.UserId == userId && x.MatrixId == matrixModule.MatrixId);

            var deadlineOverride2 = GetDueDateOverride(userId, matrixModule.Id);
            var deadline2 = MatrixModuleDateHelper.GetEffectiveDeadline(
                ma.StartDate, matrixModule.WeekNumber, matrixModule.DayOfWeek,
                ma.Matrix.RefreshIntervalDays, deadlineOverride2);

            var matrixActivityStatus = GetActivityStatus(userId, moduleId, matrixModule.Module.Category, today, deadline2);
            if (!matrixActivityStatus.CanComplete)
                throw new BadRequestException(matrixActivityStatus.BlockReason ?? "Not enough activity days.");

            _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
            {
                UserId = userId,
                MatrixModuleId = matrixModule.Id,
                CompletedDate = today
            });

            _lessonPanelService.AddActivityPoints(
                userId, 10,
                $"Completed curriculum module ({matrixModule.Module.Name})");
            _dbContext.SaveChanges();
        }

        private StudentAssignmentDto MapToStudentAssignmentDto(UserMatrixAssignment assignment,
            List<int> completedMatrixModuleIds, Dictionary<int, DateOnly> dueDateOverrides,
            DateOnly today, DateOnly currentWeekMonday)
        {
            return new StudentAssignmentDto
            {
                MatrixId = assignment.MatrixId,
                MatrixName = assignment.Matrix.Name,
                StartDate = assignment.StartDate,
                RefreshIntervalDays = assignment.Matrix.RefreshIntervalDays,
                Modules = assignment.Matrix.MatrixModules
                    .OrderBy(mm => mm.WeekNumber)
                    .ThenBy(mm => mm.DayOfWeek)
                    .Select((mm, index) => MapToStudentModuleDto(
                        mm, assignment, completedMatrixModuleIds, dueDateOverrides,
                        today, currentWeekMonday, index + 1))
                    .Where(dto => dto != null)
                    .ToList()!
            };
        }

        private StudentModuleDto? MapToStudentModuleDto(MatrixModule mm, UserMatrixAssignment assignment,
            List<int> completedMatrixModuleIds, Dictionary<int, DateOnly> dueDateOverrides,
            DateOnly today, DateOnly currentWeekMonday, int order)
        {
            var originalDeadline = MatrixModuleDateHelper.ComputeDeadline(
                assignment.StartDate, mm.WeekNumber, mm.DayOfWeek, assignment.Matrix.RefreshIntervalDays);

            var effectiveDeadline = dueDateOverrides.TryGetValue(mm.Id, out var ov) ? ov : originalDeadline;
            var unlockDate = WeekHelper.GetWeekMonday(originalDeadline);

            var isCompleted = completedMatrixModuleIds.Contains(mm.Id);
            var isOverdue = today > effectiveDeadline && !isCompleted;
            var isFutureWeek = unlockDate > currentWeekMonday;

            var userId = _userContextService.GetUserId!.Value;
            var module = _dbContext.Modules
                .Include(m => m.Presentation)
                .Include(m => m.TheaterItem)
                .FirstOrDefault(m => m.Id == mm.ModuleId) ?? mm.Module;

            return BuildModuleDto(
                id: mm.Id,
                moduleId: mm.ModuleId,
                module: module,
                order: order,
                weekNumber: mm.WeekNumber,
                dayOfWeek: mm.DayOfWeek,
                unlockDate: unlockDate,
                deadline: effectiveDeadline,
                isUnlocked: !isFutureWeek || isOverdue,
                isCompleted: isCompleted,
                isOverdue: isOverdue,
                userId: userId,
                today: today,
                url: module?.TheaterItem?.Url,
                assignedDate: unlockDate
            );
        }

        private DateOnly? GetDueDateOverride(int userId, int matrixModuleId)
        {
            return _dbContext.UserMatrixModuleDueDateOverrides
                .Where(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId)
                .Select(x => (DateOnly?)x.NewDeadline)
                .FirstOrDefault();
        }

        private StudentModuleDto BuildModuleDto(int id, int moduleId, Module module, int order,
            int weekNumber, int dayOfWeek, DateOnly unlockDate, DateOnly deadline, bool isUnlocked,
            bool isCompleted, bool isOverdue, int userId, DateOnly today, string? url, DateOnly? assignedDate = null)
        {
            var activityStatus = GetActivityStatus(userId, moduleId, module.Category, today, assignedDate);

            return new StudentModuleDto
            {
                Id = id,
                ModuleId = moduleId,
                Name = module.Name,
                Description = module.Description ?? string.Empty,
                Category = module.Category,
                Order = order,
                WeekNumber = weekNumber,
                DayOfWeek = dayOfWeek,
                UnlockDate = unlockDate,
                Deadline = deadline,
                IsUnlocked = isUnlocked,
                IsCompleted = isCompleted,
                IsOverdue = isOverdue,
                Url = url,
                ActivityDaysCount = activityStatus.Streak,
                ActivityDaysRequired = activityStatus.Required,
                CanComplete = activityStatus.CanComplete,
                CompletionBlockReason = activityStatus.BlockReason,
                PresentationUrl = module.Presentation?.Url,
                PresentationText = module.Presentation?.Text
            };
        }

        private ActivityStatus GetActivityStatus(int userId, int moduleId, string category,
                              DateOnly today, DateOnly? assignedDate = null)
        {
            const int REQUIRED_DAYS = 3;
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
                            Required = REQUIRED_DAYS,
                            CanComplete = streak >= REQUIRED_DAYS,
                            BlockReason = streak >= REQUIRED_DAYS ? null
                                : $"Study flashcards for {REQUIRED_DAYS - streak} more consecutive day(s)."
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
                            Required = REQUIRED_DAYS,
                            CanComplete = streak >= REQUIRED_DAYS,
                            BlockReason = streak >= REQUIRED_DAYS ? null
                                : $"Practice sentence flashcards for {REQUIRED_DAYS - streak} more consecutive day(s)."
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
                            Required = REQUIRED_DAYS,
                            CanComplete = streak >= REQUIRED_DAYS,
                            BlockReason = streak >= REQUIRED_DAYS ? null
                                : $"Visit Memories for {REQUIRED_DAYS - streak} more consecutive day(s)."
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
                            Required = REQUIRED_DAYS,
                            CanComplete = streak >= REQUIRED_DAYS,
                            BlockReason = streak >= REQUIRED_DAYS ? null
                                : $"Practice pronunciation for {REQUIRED_DAYS - streak} more consecutive day(s)."
                        };
                    }
                case "Sentences":
                case "Presentation":
                case "General":
                default:
                    return new ActivityStatus { Streak = 0, Required = 0, CanComplete = true, BlockReason = null };
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
                if (date == expected) { streak++; expected = expected.AddDays(-1); }
                else break;
            }
            return streak;
        }
    }
}