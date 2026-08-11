using inzBackend.Enums;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.WeeklyMoviesModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentLearningServices.WeeklyMovies
{
    public class WeeklyMoviesService : IWeeklyMoviesService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public WeeklyMoviesService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public WeeklyMoviesResponseDto GetWeeklyMoviesStats(string? timeframe = "week", string? type = "movie")
        {
            var today = PolandTime.Today;
            var weekStart = WeekHelper.GetWeekMonday(today);
            var weekEnd = weekStart.AddDays(6);

            var isAllTime = string.Equals(timeframe, "all", StringComparison.OrdinalIgnoreCase);

            var isTv = string.Equals(type, "tv", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(type, "tvseries", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(type, "series", StringComparison.OrdinalIgnoreCase);

            var targetMediaType = isTv ? MediaType.TvSeries : MediaType.Movie;

            var query = _dbContext.ListeningReports
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Where(r => r.MediaType == targetMediaType);

            if (!isAllTime)
            {
                query = query.Where(r => r.ReportDate >= weekStart && r.ReportDate <= weekEnd);
            }

            var movieReports = query.ToList();

            var totalEpisodesWatched = movieReports.Sum(r => r.EpisodeCount);

            var movieGroups = movieReports
                .Select(r => new
                {
                    Report = r,
                    CleanTitle = string.IsNullOrWhiteSpace(r.Title) ? "Untitled Movie" : r.Title.Trim()
                })
                .GroupBy(x => x.CleanTitle, StringComparer.OrdinalIgnoreCase)
                .Select(g => new
                {
                    Title = g.First().CleanTitle,
                    TotalWatchedCount = g.Sum(x => x.Report.EpisodeCount),
                    UniqueViewersCount = g.Select(x => x.Report.UserId).Distinct().Count()
                })
                .OrderByDescending(m => m.TotalWatchedCount)
                .ThenBy(m => m.Title)
                .ToList();

            var topMovies = movieGroups.Select((m, index) => new WeeklyMovieItemDto
            {
                Rank = index + 1,
                Title = m.Title,
                TotalWatchedCount = m.TotalWatchedCount,
                UniqueViewersCount = m.UniqueViewersCount
            }).ToList();

            var watcherGroups = movieReports
                .GroupBy(r => r.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    User = g.First().User,
                    TotalWatchedCount = g.Sum(x => x.EpisodeCount)
                })
                .OrderByDescending(w => w.TotalWatchedCount)
                .ThenBy(w => w.User?.Username ?? string.Empty)
                .Take(3)
                .ToList();

            var topWatchers = watcherGroups.Select((w, index) => new TopWatcherDto
            {
                Rank = index + 1,
                UserId = w.UserId,
                Username = w.User?.Username ?? $"User #{w.UserId}",
                AvatarUrl = w.User?.Profile?.AvatarUrl,
                TotalWatchedCount = w.TotalWatchedCount
            }).ToList();

            return new WeeklyMoviesResponseDto
            {
                WeekStartDate = weekStart,
                WeekEndDate = weekEnd,
                TotalEpisodesWatched = totalEpisodesWatched,
                TopMovies = topMovies,
                TopWatchers = topWatchers
            };
        }
    }
}
