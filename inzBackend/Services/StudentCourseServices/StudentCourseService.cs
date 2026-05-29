using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
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

        public StudentCourseService(GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService, ILessonPanelService lessonPanelService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _lessonPanelService = lessonPanelService;
        }

        public List<StudentAssignmentDto> getStudentsAssignments()
        {
            var userId = _userContextService.GetUserId;
            var now = PolandTime.Today;

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

            return assignments
                .Select(a => mapToStudentAssignmentDto(a, completedMatrixModuleIds, now))
                .ToList();
        }

        public void completeModule(int matrixModuleId)
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
                .FirstOrDefault(x => x.UserId == userId
                                  && x.MatrixId == matrixModule.MatrixId);

            var unlockDate = matrixAssignment is not null
                ? calculateUnlockDate(
                    matrixAssignment.StartDate,
                    matrixModule.Matrix.RefreshIntervalDays,
                    matrixModule.WeekNumber,
                    matrixModule.DayOfWeek)
                : today;

            var (_, _, canComplete, blockReason) =
                getActivityStatus(userId, matrixModule.ModuleId,
                    matrixModule.Module.Category, today, unlockDate);

            if (!canComplete)
                throw new BadRequestException(blockReason ?? "Not enough activity days.");

            _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
            {
                UserId = userId,
                MatrixModuleId = matrixModuleId,
                CompletedDate = today
            });

            _lessonPanelService.addActivityPoints(
                userId, 10, $"Completed curriculum module ({matrixModule.Module.Name})");

            _dbContext.SaveChanges();
        }

        public void uncompleteModule(int matrixModuleId)
        {
            var userId = _userContextService.GetUserId;

            var matrixModule = _dbContext.MatrixModules
                .Include(x => x.Module)
                .Include(x => x.Matrix)
                .FirstOrDefault(x => x.Id == matrixModuleId)
                ?? throw new NotFoundException("Matrix module not found");

            var completion = _dbContext.UserMatrixModuleCompletions
                .FirstOrDefault(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);

            if (completion is null)
                throw new BadRequestException($"Matrix module is not completed");

            _lessonPanelService.addActivityPoints(
                userId.Value,
                -10,
                $"Reversed completion of curriculum module ({matrixModule.Module.Name})"
            );

            _dbContext.UserMatrixModuleCompletions.Remove(completion);
            _dbContext.SaveChanges();
        }

        public List<StudentModuleDto> getSingleModules()
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var matrixModuleIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .SelectMany(x => x.Matrix.MatrixModules)
                .Select(x => x.ModuleId)
                .ToList();

            var assignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                    .ThenInclude(m => m.Presentation)
                .Include(x => x.Module)
                    .ThenInclude(x => x.TheaterItem)
                .Where(x => x.UserId == userId
                         && !matrixModuleIds.Contains(x.ModuleId)
                         && !x.IsCompleted)
                .ToList();

            return assignments.Select((x, index) => buildModuleDto(
                id: x.Id,
                moduleId: x.ModuleId,
                module: x.Module,
                order: index + 1,
                weekNumber: 0,
                dayOfWeek: 0,
                unlockDate: x.DueDate,
                isUnlocked: true,
                isCompleted: x.IsCompleted,
                isOverdue: x.DueDate < today,
                userId: userId,
                today: today,
                url: x.Module?.TheaterItem?.Url,
                assignedDate: DateOnly.FromDateTime(x.CreatedAt.DateTime)
            )).ToList();
        }

        public void completeSingleModule(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var assignment = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .FirstOrDefault(x => x.Id == id && x.UserId == userId)
                ?? throw new NotFoundException("Module assignment not found");

            var assignedDate = DateOnly.FromDateTime(assignment.CreatedAt.DateTime);

            var (_, _, canComplete, blockReason) =
                getActivityStatus(userId, assignment.ModuleId,
                    assignment.Module.Category, today, assignedDate);

            if (!canComplete)
                throw new BadRequestException(blockReason ?? "Not enough activity days.");

            assignment.IsCompleted = true;
            _lessonPanelService.addActivityPoints(
                userId, 10, $"Completed assignment {assignment.Module.Name}");
            _dbContext.SaveChanges();
        }

        public void uncompleteSingleModule(int id)
        {
            var userId = _userContextService.GetUserId;

            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            assignment.IsCompleted = false;

            _lessonPanelService.addActivityPoints(
                userId.Value,
                -10,
                $"Reversed completion of additional assignment ({assignment.Module.Name})"
            );
            _dbContext.SaveChanges();
        }

        public List<StudentModuleDto> getCompletedSingleModules()
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

        public StudentModuleDto? getStudentModule(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var directAssignment = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                    .ThenInclude(m => m.Presentation)
                .Include(x => x.Module)
                    .ThenInclude(m => m.TheaterItem)
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            if (directAssignment is not null)
            {
                return buildModuleDto(
                    id: directAssignment.Id,
                    moduleId: moduleId,
                    module: directAssignment.Module,
                    order: 1,
                    weekNumber: 0,
                    dayOfWeek: 0,
                    unlockDate: directAssignment.DueDate,
                    isUnlocked: true,
                    isCompleted: directAssignment.IsCompleted,
                    isOverdue: directAssignment.DueDate < today,
                    userId: userId,
                    today: today,
                    url: directAssignment.Module?.TheaterItem?.Url,
                    assignedDate: DateOnly.FromDateTime(directAssignment.CreatedAt.DateTime)
                );
            }

            var matrixModule = _dbContext.MatrixModules
                .Include(x => x.Module)
                    .ThenInclude(m => m.Presentation)
                .Include(x => x.Module)
                    .ThenInclude(m => m.TheaterItem)
                .Include(x => x.Matrix)
                .FirstOrDefault(x => x.ModuleId == moduleId);

            if (matrixModule is null) return null;

            var matrixAssignment = _dbContext.UserMatrixAssignments
                .FirstOrDefault(x => x.UserId == userId && x.MatrixId == matrixModule.MatrixId);

            if (matrixAssignment is null) return null;

            var unlockDate = calculateUnlockDate(
                matrixAssignment.StartDate,
                matrixModule.Matrix.RefreshIntervalDays,
                matrixModule.WeekNumber,
                matrixModule.DayOfWeek);

            var isCompleted = _dbContext.UserMatrixModuleCompletions
                .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModule.Id);

            return buildModuleDto(
                id: matrixModule.Id,
                moduleId: moduleId,
                module: matrixModule.Module,
                order: 1,
                weekNumber: matrixModule.WeekNumber,
                dayOfWeek: matrixModule.DayOfWeek,
                unlockDate: unlockDate,
                isUnlocked: today >= unlockDate,
                isCompleted: isCompleted,
                isOverdue: false,
                userId: userId,
                today: today,
                url: matrixModule.Module?.TheaterItem?.Url,
                assignedDate: unlockDate
            );
        }

        public void completeStudentModule(int moduleId)
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
                var (_, _, canComplete, blockReason) = getActivityStatus(userId, moduleId, direct.Module.Category, today, assignedDate);
                if (!canComplete) throw new BadRequestException(blockReason ?? "Not enough activity days.");

                direct.IsCompleted = true;
                _lessonPanelService.addActivityPoints(userId, 10, $"Completed assignment {direct.Module.Name}");
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
                .FirstOrDefault(x => x.ModuleId == moduleId && userMatrixIds.Contains(x.MatrixId));

            if (matrixModule is null)
                throw new NotFoundException("Module assignment not found");

            var alreadyCompleted = _dbContext.UserMatrixModuleCompletions
                .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModule.Id);

            if (alreadyCompleted) return;

            var matrixAssignment = _dbContext.UserMatrixAssignments.First(x => x.UserId == userId && x.MatrixId == matrixModule.MatrixId);
            var unlockDate = calculateUnlockDate(matrixAssignment.StartDate, matrixModule.Matrix.RefreshIntervalDays, matrixModule.WeekNumber, matrixModule.DayOfWeek);

            var (_, _, mCanComplete, mBlockReason) = getActivityStatus(userId, moduleId, matrixModule.Module.Category, today, unlockDate);
            if (!mCanComplete) throw new BadRequestException(mBlockReason ?? "Not enough activity days.");

            _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
            {
                UserId = userId,
                MatrixModuleId = matrixModule.Id,
                CompletedDate = today
            });

            _lessonPanelService.addActivityPoints(userId, 10, $"Completed curriculum module ({matrixModule.Module.Name})");
            _dbContext.SaveChanges();
        }


        private StudentAssignmentDto mapToStudentAssignmentDto(
            UserMatrixAssignment assignment,
            List<int> completedMatrixModuleIds,
            DateOnly today)
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
                    .Select((mm, index) => mapToStudentModuleDto(
                        mm, assignment, completedMatrixModuleIds, today, index + 1))
                    .ToList()
            };
        }

        private StudentModuleDto mapToStudentModuleDto(
            MatrixModule mm, UserMatrixAssignment assignment,
            List<int> completedMatrixModuleIds, DateOnly today, int order)
        {
            var unlockDate = calculateUnlockDate(
                assignment.StartDate,
                assignment.Matrix.RefreshIntervalDays,
                mm.WeekNumber,
                mm.DayOfWeek);

            var userId = _userContextService.GetUserId!.Value;
            var module = _dbContext.Modules
                .Include(m => m.Presentation)
                .Include(m => m.TheaterItem)
                .FirstOrDefault(m => m.Id == mm.ModuleId) ?? mm.Module;

            return buildModuleDto(
                id: mm.Id,
                moduleId: mm.ModuleId,
                module: module,
                order: order,
                weekNumber: mm.WeekNumber,
                dayOfWeek: mm.DayOfWeek,
                unlockDate: unlockDate,
                isUnlocked: today >= unlockDate,
                isCompleted: completedMatrixModuleIds.Contains(mm.Id),
                isOverdue: false,
                userId: userId,
                today: today,
                url: module?.TheaterItem?.Url,
                assignedDate: unlockDate
            );
        }

        private static DateOnly calculateUnlockDate(DateOnly startDate, int interval, int weekNum, int dayOfWeek)
        {
            var daysToAdd = (weekNum - 1) * interval + (dayOfWeek - 1);
            return startDate.AddDays(daysToAdd);
        }

        private StudentModuleDto buildModuleDto(
            int id, int moduleId, Module module, int order,
            int weekNumber, int dayOfWeek, DateOnly unlockDate,
            bool isUnlocked, bool isCompleted, bool isOverdue,
            int userId, DateOnly today, string? url,
            DateOnly? assignedDate = null)
        {
            var (activityDays, required, canComplete, blockReason) =
                getActivityStatus(userId, moduleId, module.Category, today, assignedDate);

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
                IsUnlocked = isUnlocked,
                IsCompleted = isCompleted,
                IsOverdue = isOverdue,
                Url = url,
                ActivityDaysCount = activityDays,
                ActivityDaysRequired = required,
                CanComplete = canComplete,
                CompletionBlockReason = blockReason,
                PresentationUrl = module.Presentation?.Url,
                PresentationText = module.Presentation?.Text
            };
        }

        private (int days, int required, bool canComplete, string? blockReason) getActivityStatus(int userId, int moduleId, string category, DateOnly today, DateOnly? assignedDate = null)
        {
            const int REQUIRED_DAYS = 3;
            var countFrom = assignedDate ?? DateOnly.MinValue;

            switch (category)
            {
                case "Flashcards":
                    {
                        var days = _dbContext.FlashcardStudyLogs
                            .Where(x => x.UserId == userId && x.StudyDate >= countFrom)
                            .Select(x => x.StudyDate)
                            .Distinct()
                            .Count();

                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Study flashcards for {REQUIRED_DAYS - days} more day(s).");
                    }

                case "SentenceFlashcards":
                    {
                        var days = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId
                                     && x.Section == "sentenceflashcards"
                                     && x.ActivityDate >= countFrom)
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .Count();

                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Practice sentence flashcards for {REQUIRED_DAYS - days} more day(s).");
                    }

                case "Memories":
                    {
                        var days = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId
                                     && x.Section == "memories"
                                     && x.ActivityDate >= countFrom)
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .Count();

                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Visit Memories for {REQUIRED_DAYS - days} more day(s).");
                    }

                case "Pronunciation":
                    {
                        var days = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId
                                     && x.Section == "pronunciation"
                                     && x.ActivityDate >= countFrom)
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .Count();

                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Practice pronunciation for {REQUIRED_DAYS - days} more day(s).");
                    }

                case "Sentences":
                case "Presentation":
                case "General":
                default:
                    return (0, 0, true, null);
            }
        }
    }
}