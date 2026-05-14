using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.StudentCourseModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentCourseServices
{
    public class StudentCourseService : IStudentCourseService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public StudentCourseService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public List<StudentAssignmentDto> getStudentsAssignments()
        {
            var userId = _userContextService.GetUserId;
            var now = DateOnly.FromDateTime(DateTime.UtcNow);

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
                CompletedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            _dbContext.SaveChanges();
        }

        public void uncompleteModule(int matrixModuleId)
        {
            var userId = _userContextService.GetUserId;

            var completion = _dbContext.UserMatrixModuleCompletions
                .FirstOrDefault(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);

            if (completion is null)
                throw new BadRequestException($"Matrix module is not completed");

            _dbContext.UserMatrixModuleCompletions.Remove(completion);
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