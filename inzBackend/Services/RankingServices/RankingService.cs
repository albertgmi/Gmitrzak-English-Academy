using inzBackend.Entities;
using inzBackend.Models.RankingModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.RankingServices
{
    public class RankingService : IRankingService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        private static readonly string[] Titles =
        [
            "Supreme Champion",
            "The Unstoppable",
            "Language Emperor",
            "Rising Titan",
            "Silent Scholar",
            "Word Warrior",
            "The Persistent",
            "Knowledge Seeker",
            "Dedicated Learner",
            "The Beginner"
        ];

        public RankingService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public RankingDto getRanking(string period)
        {
            var currentUserId = _userContextService.GetUserId!.Value;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var (dateFrom, dateTo) = getDateRange(period, today);

            var users = _dbContext.Users
                .Include(x => x.Profile)
                .Where(x => x.Role == Enums.UserRole.User && x.IsActive)
                .ToList();

            var activityPoints = _dbContext.ActivityPoints
                .Where(x => x.PointDate >= dateFrom && x.PointDate <= dateTo)
                .GroupBy(x => x.UserId)
                .Select(g => new { UserId = g.Key, Points = g.Sum(x => x.Points) })
                .ToList();

            var flashcardsDone = _dbContext.FlashcardStudyLogs
                .Where(x => x.StudyDate >= dateFrom && x.StudyDate <= dateTo)
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Done = g.Sum(x => x.EasyCount + x.HardCount + x.IncorrectCount)
                })
                .ToList();

            var grades = _dbContext.Grades
                .Where(x => x.GradeDate >= dateFrom && x.GradeDate <= dateTo)
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Average = g.Average(x => (double)x.Percentage)
                })
                .ToList();

            var reactions = _dbContext.RankingReactions
                .Where(x => x.Period == period)
                .GroupBy(x => new { x.ToUserId, x.Emoji })
                .Select(g => new { g.Key.ToUserId, g.Key.Emoji, Count = g.Count() })
                .ToList();

            var myReactions = _dbContext.RankingReactions
                .Where(x => x.FromUserId == currentUserId && x.Period == period)
                .Select(x => new { x.ToUserId, x.Emoji })
                .ToList();

            var entries = users.Select(u =>
            {
                var pts = activityPoints.FirstOrDefault(x => x.UserId == u.Id)?.Points ?? 0;
                var fc = flashcardsDone.FirstOrDefault(x => x.UserId == u.Id)?.Done ?? 0;
                var avg = (decimal)(grades.FirstOrDefault(x => x.UserId == u.Id)?.Average ?? 0);
                var score = pts + (fc * 2) + (int)(avg * 3);

                var userReactions = reactions
                    .Where(r => r.ToUserId == u.Id)
                    .ToDictionary(
                        r => r.Emoji,
                        r => r.Count
                    );

                return new RankingEntryDto
                {
                    UserId = u.Id,
                    Username = u.Username,
                    AvatarUrl = u.Profile?.AvatarUrl,
                    ActivityPoints = pts,
                    AverageGrade = Math.Round(avg, 1),
                    FlashcardsDone = fc,
                    Score = score,
                    Reactions = userReactions
                };
            })
            .OrderByDescending(x => x.Score)
            .ToList();

            for (var i = 0; i < entries.Count; i++)
            {
                entries[i].Position = i + 1;
                entries[i].Title = i < Titles.Length ? Titles[i] : "Brave Student";
            }

            foreach (var e in entries)
            {
                foreach (var emoji in new[] { "👏", "👑", "🔥" })
                {
                    if (myReactions.Any(r => r.ToUserId == e.UserId && r.Emoji == emoji))
                    {
                        e.Reactions.TryGetValue(emoji, out var cnt);
                        e.Reactions[emoji + "_me"] = 1;
                    }
                }
            }

            var currentUserEntry = entries.FirstOrDefault(x => x.UserId == currentUserId);
            var currentPos = currentUserEntry?.Position ?? 0;
            var nextEntry = currentPos > 1
                ? entries.FirstOrDefault(x => x.Position == currentPos - 1)
                : null;
            var pointsToNext = nextEntry != null
                ? nextEntry.Score - (currentUserEntry?.Score ?? 0)
                : 0;

            return new RankingDto
            {
                Period = period,
                Entries = entries,
                CurrentUserPosition = currentPos,
                PointsToNextPosition = Math.Max(0, pointsToNext),
                CurrentUserOnPodium = currentPos is >= 1 and <= 3
            };
        }

        public void addReaction(int fromUserId, AddReactionRequest request)
        {
            var validEmojis = new[] { "👏", "👑", "🔥" };
            if (!validEmojis.Contains(request.Emoji)) return;
            if (fromUserId == request.ToUserId) return;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var exists = _dbContext.RankingReactions.Any(x =>
                x.FromUserId == fromUserId &&
                x.ToUserId == request.ToUserId &&
                x.Emoji == request.Emoji &&
                x.Period == request.Period);

            if (exists) return;

            _dbContext.RankingReactions.Add(new RankingReaction
            {
                FromUserId = fromUserId,
                ToUserId = request.ToUserId,
                Emoji = request.Emoji,
                Period = request.Period,
                ReactionDate = today
            });
            _dbContext.SaveChanges();
        }

        public void removeReaction(int fromUserId, int toUserId, string emoji, string period)
        {
            var reaction = _dbContext.RankingReactions.FirstOrDefault(x =>
                x.FromUserId == fromUserId &&
                x.ToUserId == toUserId &&
                x.Emoji == emoji &&
                x.Period == period);

            if (reaction is null) return;
            _dbContext.RankingReactions.Remove(reaction);
            _dbContext.SaveChanges();
        }

        private static (DateOnly from, DateOnly to) getDateRange(string period, DateOnly today)
        {
            return period switch
            {
                "weekly" => (
                    today.AddDays(-(((int)today.DayOfWeek + 6) % 7)),
                    today
                ),
                "monthly" => (
                    new DateOnly(today.Year, today.Month, 1),
                    today
                ),
                _ => (DateOnly.MinValue, today)
            };
        }
    }
}
