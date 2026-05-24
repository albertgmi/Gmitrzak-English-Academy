using AutoMapper;
using inzBackend.Entities;
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
            var userId = _userContextService.GetUserId;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var assignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && !x.IsCompleted)
                .OrderBy(x => x.DueDate)
                .ToList();

            var dtos = _mapper.Map<List<AssignmentStudentDto>>(assignments);
            dtos.ForEach(d => d.IsOverdue = d.DueDate < today);
            return dtos;
        }

        public List<AssignmentStudentDto> getAssignmentHistory()
        {
            var userId = _userContextService.GetUserId;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var assignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && x.IsCompleted)
                .OrderByDescending(x => x.DueDate)
                .ToList();

            var dtos = _mapper.Map<List<AssignmentStudentDto>>(assignments);
            dtos.ForEach(d => d.IsOverdue = false);
            return dtos;
        }
    }
}