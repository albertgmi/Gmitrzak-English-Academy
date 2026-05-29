using AutoMapper;
using inzBackend.Entities;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.AssignmentStudentModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentLearningServices.Assignment
{
    public class StudentAssignmentService : IStudentAssignmentService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public StudentAssignmentService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }
        public List<AssignmentStudentDto> getActiveAssignments()
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;
            var result = new List<AssignmentStudentDto>();

            var directAssignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && !x.IsCompleted)
                .OrderBy(x => x.DueDate)
                .ToList();

            var directDtos = _mapper.Map<List<AssignmentStudentDto>>(directAssignments);
            directDtos.ForEach(d =>
            {
                d.IsOverdue = d.DueDate < today;
                d.HasDeadline = true;
            });
            result.AddRange(directDtos);

            var completedMatrixModuleIds = _dbContext.UserMatrixModuleCompletions
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixModuleId)
                .ToList();

            var matrixAssignments = _dbContext.UserMatrixAssignments
                .Include(x => x.Matrix)
                    .ThenInclude(m => m.MatrixModules)
                        .ThenInclude(mm => mm.Module)
                .Where(x => x.UserId == userId)
                .ToList();

            foreach (var ma in matrixAssignments)
            {
                foreach (var mm in ma.Matrix.MatrixModules)
                {
                    if (completedMatrixModuleIds.Contains(mm.Id)) continue;

                    var unlockDate = ma.StartDate
                        .AddDays((mm.WeekNumber - 1) * ma.Matrix.RefreshIntervalDays)
                        .AddDays(mm.DayOfWeek - 1);

                    if (unlockDate > today) continue;

                    if (directAssignments.Any(d => d.ModuleId == mm.ModuleId)) continue;

                    result.Add(new AssignmentStudentDto
                    {
                        Id = mm.Id,
                        ModuleId = mm.ModuleId,
                        ModuleName = mm.Module.Name,
                        ModuleDescription = mm.Module.Description ?? string.Empty,
                        Category = mm.Module.Category,
                        DueDate = unlockDate,
                        IsCompleted = false,
                        IsOverdue = false,
                        HasDeadline = false,
                        IsFromMatrix = true,
                        MatrixName = ma.Matrix.Name
                    });
                }
            }

            return result
                .OrderByDescending(x => x.IsOverdue)
                .ThenBy(x => x.HasDeadline ? x.DueDate : DateOnly.MaxValue)
                .ToList();
        }

        public List<AssignmentStudentDto> getAssignmentHistory()
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var directCompleted = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && x.IsCompleted)
                .OrderByDescending(x => x.DueDate)
                .ToList();

            var directDtos = _mapper.Map<List<AssignmentStudentDto>>(directCompleted);
            directDtos.ForEach(d => d.IsOverdue = false);

            var completedMatrixModuleIds = _dbContext.UserMatrixModuleCompletions
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixModuleId)
                .ToList();

            var matrixCompleted = new List<AssignmentStudentDto>();

            if (completedMatrixModuleIds.Any())
            {
                var matrixAssignments = _dbContext.UserMatrixAssignments
                    .Include(x => x.Matrix)
                        .ThenInclude(m => m.MatrixModules)
                            .ThenInclude(mm => mm.Module)
                    .Where(x => x.UserId == userId)
                    .ToList();

                foreach (var ma in matrixAssignments)
                {
                    foreach (var mm in ma.Matrix.MatrixModules
                        .Where(mm => completedMatrixModuleIds.Contains(mm.Id)))
                    {
                        var unlockDate = ma.StartDate
                            .AddDays((mm.WeekNumber - 1) * ma.Matrix.RefreshIntervalDays)
                            .AddDays(mm.DayOfWeek - 1);

                        matrixCompleted.Add(new AssignmentStudentDto
                        {
                            Id = mm.Id,
                            ModuleId = mm.ModuleId,
                            ModuleName = mm.Module.Name,
                            ModuleDescription = mm.Module.Description ?? string.Empty,
                            Category = mm.Module.Category,
                            DueDate = unlockDate,
                            IsCompleted = true,
                            IsOverdue = false,
                            IsFromMatrix = true,
                            MatrixName = ma.Matrix.Name
                        });
                    }
                }
            }

            return directDtos
                .Concat(matrixCompleted)
                .OrderByDescending(x => x.DueDate)
                .ToList();
        }
    }
}