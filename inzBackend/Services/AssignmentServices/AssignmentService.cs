using AutoMapper;
using inzBackend.Entities;
using inzBackend.Exceptions;
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

        public List<MatrixAssignmentDto> getAllMatrixAssignments()
        {
            return _dbContext.UserMatrixAssignments
                .Include(x => x.User)
                .Include(x => x.Matrix)
                    .ThenInclude(x => x.MatrixModules)
                        .ThenInclude(x => x.Module)
                .ToList()
                .Select(x => mapToMatrixAssignmentDto(x))
                .ToList();
        }

        public List<MatrixAssignmentDto> getMatrixAssignmentsByUser(int userId)
        {
            return _dbContext.UserMatrixAssignments
                .Include(x => x.User)
                .Include(x => x.Matrix)
                    .ThenInclude(x => x.MatrixModules)
                        .ThenInclude(x => x.Module)
                .Where(x => x.UserId == userId)
                .ToList()
                .Select(x => mapToMatrixAssignmentDto(x))
                .ToList();
        }

        public void createMatrixAssignment(CreateMatrixAssignmentRequest request)
        {
            var userExists = _dbContext.Users.Any(x => x.Id == request.UserId);
            if (!userExists)
                throw new NotFoundException("User not found");

            var matrixExists = _dbContext.Matrices.Any(x => x.Id == request.MatrixId);
            if (!matrixExists)
                throw new NotFoundException("Matrix not found");

            var alreadyAssigned = _dbContext.UserMatrixAssignments
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

        public void deleteMatrixAssignment(int id)
        {
            var assignment = _dbContext.UserMatrixAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Matrix assignment not found");

            _dbContext.UserMatrixAssignments.Remove(assignment);
            _dbContext.SaveChanges();
        }

        public List<ModuleAssignmentDto> getAllModuleAssignments()
        {
            return _dbContext.UserModuleAssignments
                .Include(x => x.User)
                .Include(x => x.Module)
                .ToList()
                .Select(x => mapToModuleAssignmentDto(x))
                .ToList();
        }

        public List<ModuleAssignmentDto> getModuleAssignmentsByUser(int userId)
        {
            return _dbContext.UserModuleAssignments
                .Include(x => x.User)
                .Include(x => x.Module)
                .Where(x => x.UserId == userId)
                .ToList()
                .Select(x => mapToModuleAssignmentDto(x))
                .ToList();
        }

        public void createModuleAssignment(CreateModuleAssignmentRequest request)
        {
            var userExists = _dbContext.Users.Any(x => x.Id == request.UserId);
            if (!userExists)
                throw new NotFoundException("User not found");

            var moduleExists = _dbContext.Modules.Any(x => x.Id == request.ModuleId);
            if (!moduleExists)
                throw new NotFoundException("Module not found");

            var assignment = new UserModuleAssignment
            {
                UserId = request.UserId,
                ModuleId = request.ModuleId,
                DueDate = DateOnly.Parse(request.DueDate),
                IsCompleted = false
            };

            _dbContext.UserModuleAssignments.Add(assignment);
            _dbContext.SaveChanges();
        }

        public void deleteModuleAssignment(int id)
        {
            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            _dbContext.UserModuleAssignments.Remove(assignment);
            _dbContext.SaveChanges();
        }

        public void completeModuleAssignment(int id)
        {
            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            assignment.IsCompleted = true;
            _dbContext.SaveChanges();
        }

        public void uncompleteModuleAssignment(int id)
        {
            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == id);

            if (assignment is null)
                throw new NotFoundException("Module assignment not found");

            assignment.IsCompleted = false;
            _dbContext.SaveChanges();
        }

        private MatrixAssignmentDto mapToMatrixAssignmentDto(UserMatrixAssignment x)
        {
            var completedModuleIds = _dbContext.UserModuleAssignments
                .Where(uma => uma.UserId == x.UserId && uma.IsCompleted)
                .Select(uma => uma.ModuleId)
                .ToList();

            var modules = x.Matrix.MatrixModules
                .OrderBy(mm => mm.WeekNumber)
                .ThenBy(mm => mm.DayOfWeek)
                .Select(mm => mapToModuleUnlockDto(
                    mm,
                    x.StartDate,
                    x.Matrix.RefreshIntervalDays,
                    completedModuleIds.Contains(mm.ModuleId)
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

        private static ModuleUnlockDto mapToModuleUnlockDto(MatrixModule mm, DateOnly startDate,
            int refreshIntervalDays, bool isCompleted)
        {
            var unlockDate = startDate
                .AddDays((mm.WeekNumber - 1) * refreshIntervalDays)
                .AddDays(mm.DayOfWeek - 1);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return new ModuleUnlockDto
            {
                ModuleId = mm.ModuleId,
                ModuleName = mm.Module.Name,
                ModuleDescription = mm.Module.Description,
                WeekNumber = mm.WeekNumber,
                DayOfWeek = mm.DayOfWeek,
                UnlockDate = unlockDate,
                IsUnlocked = unlockDate <= today,
                IsCompleted = isCompleted
            };
        }

        private static ModuleAssignmentDto mapToModuleAssignmentDto(UserModuleAssignment x)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

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
