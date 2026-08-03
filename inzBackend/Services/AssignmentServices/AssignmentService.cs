using AutoMapper;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.Curriculum;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AssignmentModels;
using inzBackend.Models.CourseModels;
using inzBackend.Models.MatrixAssignmentModels;
using inzBackend.Models.ModuleAssignmentModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.AssignmentServices
{
    public class AssignmentService : IAssignmentService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AssignmentService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MatrixAssignmentDto> GetAllMatrixAssignments()
        {
            return _dbContext.UserMatrixAssignments
                .Include(x => x.User)
                .Include(x => x.Matrix)
                    .ThenInclude(x => x.MatrixModules)
                        .ThenInclude(x => x.Module)
                .ToList()
                .Select(x => MapToMatrixAssignmentDto(x))
                .ToList();
        }

        public List<MatrixAssignmentDto> GetMatrixAssignmentsByUser(int userId)
        {
            return _dbContext.UserMatrixAssignments
                .Include(x => x.User)
                .Include(x => x.Matrix)
                    .ThenInclude(x => x.MatrixModules)
                        .ThenInclude(x => x.Module)
                .Where(x => x.UserId == userId)
                .ToList()
                .Select(x => MapToMatrixAssignmentDto(x))
                .ToList();
        }

        public BulkAssignmentResultDto CreateBulkMatrixAssignment(CreateBulkMatrixAssignmentRequest request)
        {
            var matrix = _dbContext.Matrices.FirstOrDefault(x => x.Id == request.MatrixId);
            if (matrix is null)
                throw new NotFoundException("Matrix not found");

            var startDate = DateOnly.Parse(request.StartDate);
            var result = new BulkAssignmentResultDto();

            foreach (var userId in request.UserIds.Distinct())
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
                if (user is null)
                {
                    result.Skipped.Add($"User #{userId}: not found");
                    continue;
                }

                var alreadyAssigned = _dbContext.UserMatrixAssignments
                    .Any(x => x.UserId == userId && x.MatrixId == request.MatrixId);
                if (alreadyAssigned)
                {
                    result.Skipped.Add($"{user.Username}: matrix \"{matrix.Name}\" already assigned");
                    continue;
                }

                _dbContext.UserMatrixAssignments.Add(new UserMatrixAssignment
                {
                    UserId = userId,
                    MatrixId = request.MatrixId,
                    StartDate = startDate
                });
                result.AssignedUsernames.Add(user.Username);
            }

            _dbContext.SaveChanges();
            return result;
        }

        public BulkAssignmentResultDto CreateCourseAssignment(CreateCourseAssignmentRequest request)
        {
            var course = _dbContext.Courses
                .Include(c => c.CourseMatrices)
                    .ThenInclude(cm => cm.Matrix)
                .FirstOrDefault(x => x.Id == request.CourseId);
            if (course is null)
                throw new NotFoundException("Course not found");

            var matrices = course.CourseMatrices.Select(cm => cm.Matrix).ToList();
            if (!matrices.Any())
                throw new BadRequestException("This course has no matrices assigned to it");

            var startDate = DateOnly.Parse(request.StartDate);
            var result = new BulkAssignmentResultDto();

            foreach (var userId in request.UserIds.Distinct())
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
                if (user is null)
                {
                    result.Skipped.Add($"User #{userId}: not found");
                    continue;
                }

                var assignedAnyMatrix = false;

                foreach (var matrix in matrices)
                {
                    var alreadyAssigned = _dbContext.UserMatrixAssignments
                        .Any(x => x.UserId == userId && x.MatrixId == matrix.Id);
                    if (alreadyAssigned)
                    {
                        result.Skipped.Add($"{user.Username}: \"{matrix.Name}\" already assigned");
                        continue;
                    }

                    _dbContext.UserMatrixAssignments.Add(new UserMatrixAssignment
                    {
                        UserId = userId,
                        MatrixId = matrix.Id,
                        StartDate = startDate
                    });
                    assignedAnyMatrix = true;
                }

                if (assignedAnyMatrix)
                    result.AssignedUsernames.Add(user.Username);
            }

            _dbContext.SaveChanges();
            return result;
        }

        public void DeleteMatrixAssignment(int id)
        {
            var assignment = _dbContext.UserMatrixAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Matrix assignment not found");

            _dbContext.UserMatrixAssignments.Remove(assignment);
            _dbContext.SaveChanges();
        }

        public List<ModuleAssignmentDto> GetAllModuleAssignments()
        {
            return _dbContext.UserModuleAssignments
                .Include(x => x.User)
                .Include(x => x.Module)
                .ToList()
                .Select(x => MapToModuleAssignmentDto(x))
                .ToList();
        }

        public List<ModuleAssignmentDto> GetModuleAssignmentsByUser(int userId)
        {
            return _dbContext.UserModuleAssignments
                .Include(x => x.User)
                .Include(x => x.Module)
                .Where(x => x.UserId == userId)
                .ToList()
                .Select(x => MapToModuleAssignmentDto(x))
                .ToList();
        }

        public void CreateModuleAssignment(CreateModuleAssignmentRequest request)
        {
            var userExists = _dbContext.Users.Any(x => x.Id == request.UserId);
            if (!userExists)
                throw new NotFoundException("User not found");

            var moduleExists = _dbContext.Modules.Any(x => x.Id == request.ModuleId);
            if (!moduleExists)
                throw new NotFoundException("Module not found");

            var parsedDueDate = DateOnly.Parse(request.DueDate);

            var assignment = new UserModuleAssignment
            {
                UserId = request.UserId,
                ModuleId = request.ModuleId,
                DueDate = parsedDueDate,
                IsCompleted = false
            };

            _dbContext.UserModuleAssignments.Add(assignment);
            _dbContext.SaveChanges();
        }

        public void DeleteModuleAssignment(int id)
        {
            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            _dbContext.UserModuleAssignments.Remove(assignment);
            _dbContext.SaveChanges();
        }

        public void CompleteModuleAssignment(int id)
        {
            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            assignment.IsCompleted = true;
            _dbContext.SaveChanges();
        }

        public void UncompleteModuleAssignment(int id)
        {
            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            assignment.IsCompleted = false;
            _dbContext.SaveChanges();
        }

        private MatrixAssignmentDto MapToMatrixAssignmentDto(UserMatrixAssignment x)
        {
            var matrixModuleIds = x.Matrix.MatrixModules.Select(mm => mm.Id).ToList();

            var completedMatrixModuleIds = _dbContext.UserMatrixModuleCompletions
                .Where(c => c.UserId == x.UserId && matrixModuleIds.Contains(c.MatrixModuleId))
                .Select(c => c.MatrixModuleId)
                .ToHashSet();

            var dueDateOverrides = _dbContext.UserMatrixModuleDueDateOverrides
                .Where(o => o.UserId == x.UserId && matrixModuleIds.Contains(o.MatrixModuleId))
                .ToDictionary(o => o.MatrixModuleId, o => o.NewDeadline);

            var modules = x.Matrix.MatrixModules
                .OrderBy(mm => mm.WeekNumber)
                .ThenBy(mm => mm.DayOfWeek)
                .Select(mm => MapToModuleUnlockDto(
                    mm,
                    x.StartDate,
                    x.Matrix.RefreshIntervalDays,
                    completedMatrixModuleIds.Contains(mm.Id),
                    dueDateOverrides.TryGetValue(mm.Id, out var ov) ? ov : (DateOnly?)null
                ))
                .ToList();

            return new MatrixAssignmentDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Username = x.User.Username,
                MatrixId = x.MatrixId,
                MatrixName = x.Matrix.Name,
                RefreshIntervalDays = x.Matrix.RefreshIntervalDays,
                StartDate = x.StartDate,
                Modules = modules
            };
        }

        private static ModuleUnlockDto MapToModuleUnlockDto(MatrixModule mm, DateOnly startDate, int refreshIntervalDays, bool isCompleted, DateOnly? deadlineOverride)
        {
            var deadline = MatrixModuleDateHelper.ComputeDeadline(
                startDate, mm.WeekNumber, mm.DayOfWeek, refreshIntervalDays);

            var unlockDate = WeekHelper.GetWeekMonday(deadline);
            var effectiveDeadline = deadlineOverride ?? deadline;
            var today = PolandTime.Today;
            var currentWeekMonday = WeekHelper.GetWeekMonday(today);

            var isFutureWeek = unlockDate > currentWeekMonday;

            return new ModuleUnlockDto
            {
                MatrixModuleId = mm.Id,
                ModuleId = mm.ModuleId,
                ModuleName = mm.Module.Name,
                ModuleDescription = mm.Module.Description,
                WeekNumber = mm.WeekNumber,
                DayOfWeek = mm.DayOfWeek,
                UnlockDate = unlockDate,
                Deadline = effectiveDeadline,
                IsUnlocked = !isFutureWeek,
                IsCompleted = isCompleted
            };
        }

        private static ModuleAssignmentDto MapToModuleAssignmentDto(UserModuleAssignment x)
        {
            var today = PolandTime.Today;

            return new ModuleAssignmentDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Username = x.User.Username,
                ModuleId = x.ModuleId,
                ModuleName = x.Module.Name,
                ModuleDescription = x.Module.Description,
                DueDate = x.DueDate,
                IsCompleted = x.IsCompleted,
                IsOverdue = x.DueDate < today && !x.IsCompleted
            };
        }
    }
}
