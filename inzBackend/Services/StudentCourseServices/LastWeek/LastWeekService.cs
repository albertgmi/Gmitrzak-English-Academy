using inzBackend.Models.StudentCourseModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using inzBackend.Helpers;

namespace inzBackend.Services.StudentCourseServices.LastWeek
{
    public class LastWeekService : ILastWeekService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public LastWeekService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public LastWeekDto getLastWeek()
        {
            var userId = _userContextService.GetUserId;
            var today = PolandTime.Today;

            var daysFromMonday = ((int)PolandTime.DateTimeNow.DayOfWeek + 6) % 7;
            var weekStart = today.AddDays(-daysFromMonday - 7);
            var weekEnd = weekStart.AddDays(6);

            var activityPoints = _dbContext.ActivityPoints
                .Where(x => x.UserId == userId && x.PointDate >= weekStart && x.PointDate <= weekEnd)
                .Sum(x => (int?)x.Points) ?? 0;

            var flashcardLogs = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId && x.StudyDate >= weekStart && x.StudyDate <= weekEnd)
                .ToList();

            var flashcardsStudied = flashcardLogs.Sum(x => x.EasyCount + x.HardCount + x.IncorrectCount);
            var flashcardTimeSeconds = flashcardLogs.Sum(x => x.TimeSpentSeconds);

            var listeningEpisodes = _dbContext.ListeningReports
                .Where(x => x.UserId == userId && x.ReportDate >= weekStart && x.ReportDate <= weekEnd)
                .Sum(x => (int?)x.EpisodeCount) ?? 0;

            var grades = _dbContext.Grades
                .Where(x => x.UserId == userId && x.GradeDate >= weekStart && x.GradeDate <= weekEnd)
                .Select(x => new GradeDto
                {
                    Id = x.Id,
                    GradeDate = x.GradeDate,
                    Percentage = x.Percentage,
                    Category = x.Category,
                    Notes = x.Notes
                })
                .ToList();

            var criteriaMet = activityPoints >= 500
                           && flashcardsStudied >= 50
                           && listeningEpisodes >= 1;

            return new LastWeekDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                TotalActivityPoints = activityPoints,
                FlashcardsStudied = flashcardsStudied,
                FlashcardTimeSeconds = flashcardTimeSeconds,
                ListeningEpisodesWatched = listeningEpisodes,
                GradesThisWeek = grades,
                RankingCriteriaMet = criteriaMet
            };
        }
    }
}
