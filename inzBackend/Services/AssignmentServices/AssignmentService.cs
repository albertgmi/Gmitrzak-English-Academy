using AutoMapper;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.Curriculum;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AssignmentModels;
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

        public void CreateMatrixAssignment(CreateMatrixAssignmentRequest request)
        {
            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.Id == request.UserId);
            if (user is null)
                throw new NotFoundException("User not found");

            var matrixExists = _dbContext
                .Matrices
                .Any(x => x.Id == request.MatrixId);
            if (!matrixExists)
                throw new NotFoundException("Matrix not found");
            // TODO maile
            var alreadyAssigned = _dbContext
                .UserMatrixAssignments
                .Any(x => x.UserId == request.UserId && x.MatrixId == request.MatrixId);
            if (alreadyAssigned)
                throw new BadRequestException("This matrix is already assigned to this user");

            var assignment = new UserMatrixAssignment
            {
                UserId = request.UserId,
                MatrixId = request.MatrixId,
                StartDate = DateOnly.Parse(request.StartDate)
            };

            _dbContext.UserMatrixAssignments.Add(assignment);
            _dbContext.SaveChanges();
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
