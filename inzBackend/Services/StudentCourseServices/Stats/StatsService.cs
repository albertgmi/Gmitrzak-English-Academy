using inzBackend.Models.StudentCourseModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using inzBackend.Helpers;

namespace inzBackend.Services.StudentCourseServices.Stats
{
    public class StatsService : IStatsService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public StatsService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public StatsDto GetStats()
        {
            var userId = _userContextService.GetUserId;
            var today = PolandTime.Today;
            var last30Days = today.AddDays(-30);

            var dailyActivity = _dbContext.ActivityPoints
                .Where(x => x.UserId == userId && x.PointDate >= last30Days)
                .GroupBy(x => x.PointDate)
                .Select(g => new DailyActivityDto
                {
                    Date = g.Key,
                    Points = g.Sum(x => x.Points)
                })
                .OrderBy(x => x.Date)
                .ToList();

            var dailyFlashcards = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId && x.StudyDate >= last30Days)
                .GroupBy(x => x.StudyDate)
                .Select(g => new DailyFlashcardsDto
                {
                    Date = g.Key,
                    CardsStudied = g.Sum(x => x.EasyCount + x.HardCount + x.IncorrectCount),
                    TimeSpentSeconds = g.Sum(x => x.TimeSpentSeconds)
                })
                .OrderBy(x => x.Date)
                .ToList();

            var grades = _dbContext.Grades
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.GradeDate)
                .Select(x => new GradeDto
                {
                    Id = x.Id,
                    GradeDate = x.GradeDate,
                    Percentage = x.Percentage,
                    Category = x.Category,
                    Notes = x.Notes
                })
                .ToList();

            var categoryBreakdown = new CategoryBreakdownDto
            {
                AvgVocabulary = grades.Where(x => x.Category == "Vocabulary").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
                AvgSentences = grades.Where(x => x.Category == "Sentences").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
                AvgMemories = grades.Where(x => x.Category == "Memories").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
                AvgPronunciation = grades.Where(x => x.Category == "Pronunciation").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
            };

            return new StatsDto
            {
                DailyActivity = dailyActivity,
                DailyFlashcards = dailyFlashcards,
                GradeHistory = grades,
                CategoryBreakdown = categoryBreakdown
            };
        }
    }
}
