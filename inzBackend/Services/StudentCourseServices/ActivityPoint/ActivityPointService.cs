using inzBackend.Models.StudentCourseModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;

namespace inzBackend.Services.StudentCourseServices
{
    public class ActivityPointService : IActivityPointService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public ActivityPointService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public ActivityPointsHistoryDto getHistory()
        {
            var userId = _userContextService.GetUserId;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var daysFromMonday = ((int)DateTime.UtcNow.DayOfWeek + 6) % 7;
            var thisWeekStart = today.AddDays(-daysFromMonday);
            var lastWeekStart = thisWeekStart.AddDays(-7);
            var lastWeekEnd = thisWeekStart.AddDays(-1);

            var all = _dbContext.ActivityPoints
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.PointDate)
                .ToList();

            return new ActivityPointsHistoryDto
            {
                TotalAllTime = all.Sum(x => x.Points),
                TotalThisWeek = all.Where(x => x.PointDate >= thisWeekStart).Sum(x => x.Points),
                TotalLastWeek = all.Where(x => x.PointDate >= lastWeekStart && x.PointDate <= lastWeekEnd).Sum(x => x.Points),
                History = all.Select(x => new ActivityPointDto
                {
                    Id = x.Id,
                    PointDate = x.PointDate,
                    Points = x.Points,
                    Reason = x.Reason
                }).ToList()
            };
        }
    }
}
