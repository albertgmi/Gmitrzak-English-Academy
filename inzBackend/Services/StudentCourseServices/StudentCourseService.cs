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
            var userId = _userContextService.GetUserId;

            var alreadyCompleted = _dbContext.UserMatrixModuleCompletions
                .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);

            if (alreadyCompleted)
                throw new BadRequestException($"Matrix module is already completed");

            _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
            {
                UserId = userId!.Value,
                MatrixModuleId = matrixModuleId,
                CompletedDate = PolandTime.Today
            });

            _lessonPanelService.addActivityPoints(
                userId.Value,
                10,
                $"Completed curriculum module (ID: {matrixModuleId})"
            );

            _dbContext.SaveChanges();
        }

        public void uncompleteModule(int matrixModuleId)
        {
            var userId = _userContextService.GetUserId;

            var completion = _dbContext.UserMatrixModuleCompletions
                .FirstOrDefault(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);

            if (completion is null)
                throw new BadRequestException($"Matrix module is not completed");

            _lessonPanelService.addActivityPoints(
                userId.Value,
                -10,
                $"Reversed completion of curriculum module (ID: {matrixModuleId})"
            );

            _dbContext.UserMatrixModuleCompletions.Remove(completion);
            _dbContext.SaveChanges();
        }

        public List<StudentModuleDto> getSingleModules()
        {
            var userId = _userContextService.GetUserId;
            var today = PolandTime.Today;

            var matrixModuleIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .SelectMany(x => x.Matrix.MatrixModules.Select(mm => new
                {
                    mm.ModuleId,
                    UnlockDate = x.StartDate.AddDays(
                        (mm.WeekNumber - 1) * x.Matrix.RefreshIntervalDays +
                        (mm.DayOfWeek - 1))
                }))
                .Where(x => x.UnlockDate >= today)
                .Select(x => x.ModuleId)
                .Distinct()
                .ToList();

            var assignments = _dbContext.UserModuleAssignments
                .Where(x => x.UserId == userId
                         && !matrixModuleIds.Contains(x.ModuleId)
                         && !x.IsCompleted)
                .Include(x => x.Module)
                    .ThenInclude(m => m.TheaterItem)
                .ToList();

            return assignments.Select((x, index) => new StudentModuleDto
                {
                    Id = x.Id,
                    ModuleId = x.ModuleId,
                    Name = x.Module.Name,
                    Description = x.Module.Description ?? string.Empty,
                    Category = x.Module.Category,
                    Order = index + 1,
                    WeekNumber = 0,
                    DayOfWeek = 0,
                    UnlockDate = x.DueDate,
                    IsUnlocked = true,
                    IsCompleted = x.IsCompleted,
                    IsOverdue = x.DueDate < today,
                    Url = x.Module.TheaterItem?.Url ?? string.Empty
                })
                .ToList();
        }

        public void completeSingleModule(int id)
        {
            var userId = _userContextService.GetUserId;

            var assignment = _dbContext
                .UserModuleAssignments
                .Include(x => x.Module)
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            assignment.IsCompleted = true;

            _lessonPanelService.addActivityPoints(
                userId.Value,
                10,
                $"Completed assignment {assignment.Module.Name}"
            );
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
                $"Reversed completion of additional assignment (ID: {id})"
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
            MatrixModule mm,
            UserMatrixAssignment assignment,
            List<int> completedMatrixModuleIds,
            DateOnly today,
            int order)
        {
            var unlockDate = calculateUnlockDate(
                assignment.StartDate,
                assignment.Matrix.RefreshIntervalDays,
                mm.WeekNumber,
                mm.DayOfWeek);

            return new StudentModuleDto
            {
                Id = mm.Id,
                ModuleId = mm.ModuleId,
                Name = mm.Module.Name,
                Description = mm.Module.Description,
                Category = mm.Module.Category,
                Order = order,
                WeekNumber = mm.WeekNumber,
                DayOfWeek = mm.DayOfWeek,
                UnlockDate = unlockDate,
                IsUnlocked = today >= unlockDate,
                IsCompleted = completedMatrixModuleIds.Contains(mm.Id)
            };
        }

        private static DateOnly calculateUnlockDate(
            DateOnly startDate,
            int interval,
            int weekNum,
            int dayOfWeek)
        {
            var daysToAdd = (weekNum - 1) * interval + (dayOfWeek - 1);
            return startDate.AddDays(daysToAdd);
        }
    }
}