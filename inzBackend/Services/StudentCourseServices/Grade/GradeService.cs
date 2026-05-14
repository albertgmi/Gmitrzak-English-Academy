using inzBackend.Models.StudentCourseModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using AutoMapper;

namespace inzBackend.Services.StudentCourseServices
{
    public class GradesService : IGradesService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public GradesService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public List<GradeDto> getGrades()
        {
            var userId = _userContextService.GetUserId;

            var grades =_dbContext
                .Grades
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.GradeDate)
                .ToList();

            return _mapper.Map<List<GradeDto>>(grades);
        }
    }
}
