using inzBackend.Models.DashboardModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.DashboardServices
{
    public class DashboardService : IDashboardService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public DashboardService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public AdminDashboardDto getAdminDashboard()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var daysFromMonday = ((int)DateTime.UtcNow.DayOfWeek + 6) % 7;
            var weekStart = today.AddDays(-daysFromMonday);

            var totalStudents = _dbContext.Users
                .Count(x => x.Role == Enums.UserRole.User && x.IsActive);

            var activeStudentsThisWeek = _dbContext.UserLoginLogs
                .Where(x => x.LoginDate >= weekStart)
                .Select(x => x.UserId)
                .Distinct()
                .Count();

            var totalFlashcards = _dbContext.Flashcards.Count();

            var totalPending = _dbContext.UserModuleAssignments
                .Count(x => !x.IsCompleted && x.DueDate >= today);

            var recentGrades = _dbContext.Grades
                .Include(x => x.User)
                .OrderByDescending(x => x.GradeDate)
                .Take(8)
                .Select(x => new RecentGradeDto
                {
                    Username = x.User.Username,
                    Category = x.Category,
                    Percentage = x.Percentage,
                    GradeDate = x.GradeDate
                })
                .ToList();

            var upcomingAssignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => !x.IsCompleted && x.DueDate >= today)
                .OrderBy(x => x.DueDate)
                .Take(8)
                .Select(x => new UpcomingAssignmentDto
                {
                    Id = x.Id,
                    ModuleName = x.Module.Name,
                    DueDate = x.DueDate,
                    IsOverdue = false
                })
                .ToList();

            var allPoints = _dbContext.ActivityPoints
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalPoints = g.Sum(x => x.Points),
                    ThisWeek = g.Where(x => x.PointDate >= weekStart).Sum(x => x.Points)
                })
                .OrderByDescending(x => x.ThisWeek)
                .Take(5)
                .ToList();

            var userIds = allPoints.Select(x => x.UserId).ToList();
            var users = _dbContext.Users
                .Where(x => userIds.Contains(x.Id))
                .ToDictionary(x => x.Id, x => x.Username);

            var topStudents = allPoints.Select(x => new StudentPointsDto
            {
                Username = users.GetValueOrDefault(x.UserId, "Unknown"),
                TotalPoints = x.TotalPoints,
                ThisWeek = x.ThisWeek
            }).ToList();

            return new AdminDashboardDto
            {
                TotalStudents = totalStudents,
                ActiveStudentsThisWeek = activeStudentsThisWeek,
                TotalFlashcards = totalFlashcards,
                TotalAssignmentsPending = totalPending,
                RecentGrades = recentGrades,
                UpcomingAssignments = upcomingAssignments,
                TopStudentsByPoints = topStudents
            };
        }

        public StudentDashboardDto getStudentDashboard()
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var daysFromMonday = ((int)DateTime.UtcNow.DayOfWeek + 6) % 7;
            var weekStart = today.AddDays(-daysFromMonday - 7);
            var weekEnd = weekStart.AddDays(6);

            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

            var totalPoints = _dbContext.ActivityPoints
                .Where(x => x.UserId == userId)
                .Sum(x => (int?)x.Points) ?? 0;

            var dueToday = _dbContext.Flashcards
                .Count(x => x.UserId == userId && x.NextReviewDate <= today);

            var studiedTodayCount = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId && x.StudyDate == today)
                .Select(x => x.FlashcardId)
                .Distinct()
                .Count();

            var activeAssignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == userId && !x.IsCompleted && x.DueDate >= today)
                .OrderBy(x => x.DueDate)
                .Take(5)
                .Select(x => new UpcomingAssignmentDto
                {
                    Id = x.Id,
                    ModuleName = x.Module.Name,
                    DueDate = x.DueDate,
                    IsOverdue = x.DueDate < today
                })
                .ToList();

            var completedMatrixModuleIds = _dbContext.UserMatrixModuleCompletions
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixModuleId)
                .ToList();

            var assignments = _dbContext.UserMatrixAssignments
                .Include(x => x.Matrix)
                    .ThenInclude(m => m.MatrixModules)
                        .ThenInclude(mm => mm.Module)
                .Where(x => x.UserId == userId)
                .ToList();

            var upcomingModules = assignments
                .SelectMany(a => a.Matrix.MatrixModules.Select(mm => new
                {
                    MatrixModuleId = mm.Id,
                    ModuleName = mm.Module.Name,
                    MatrixName = a.Matrix.Name,
                    UnlockDate = a.StartDate
                        .AddDays((mm.WeekNumber - 1) * a.Matrix.RefreshIntervalDays)
                        .AddDays(mm.DayOfWeek - 1)
                }))
                .Where(x => x.UnlockDate >= today
                         && !completedMatrixModuleIds.Contains(x.MatrixModuleId))
                .OrderBy(x => x.UnlockDate)
                .Take(5)
                .Select(x => new UpcomingModuleDto
                {
                    ModuleName = x.ModuleName,
                    MatrixName = x.MatrixName,
                    UnlockDate = x.UnlockDate,
                    IsUnlocked = x.UnlockDate <= today
                })
                .ToList();

            var agenda = _dbContext.Agendas.FirstOrDefault(x => x.UserId == userId);

            var lastWeekPoints = _dbContext.ActivityPoints
                .Where(x => x.UserId == userId
                         && x.PointDate >= weekStart
                         && x.PointDate <= weekEnd)
                .Sum(x => (int?)x.Points) ?? 0;

            var lastWeekFlashcards = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId
                         && x.StudyDate >= weekStart
                         && x.StudyDate <= weekEnd)
                .Sum(x => (int?)(x.EasyCount + x.HardCount + x.IncorrectCount)) ?? 0;

            var lastWeekListening = _dbContext.ListeningReports
                .Where(x => x.UserId == userId
                         && x.ReportDate >= weekStart
                         && x.ReportDate <= weekEnd)
                .Sum(x => (int?)x.EpisodeCount) ?? 0;

            var criteriaMet = lastWeekPoints >= (agenda?.ActivityPointTarget ?? 500)
                           && lastWeekFlashcards >= (agenda?.FlashcardTarget ?? 50)
                           && lastWeekListening >= (agenda?.ListeningEpisodeTarget ?? 1);

            var loginDates = _dbContext.UserLoginLogs
                .Where(x => x.UserId == userId)
                .Select(x => x.LoginDate)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            var streak = 0;
            var check = today;

            foreach (var date in loginDates)
            {
                if (date == check)
                {
                    streak++;
                    check = check.AddDays(-1);
                }
                else if (date < check)
                {
                    break;
                }
            }

            return new StudentDashboardDto
            {
                Username = user?.Username ?? string.Empty,
                TotalActivityPoints = totalPoints,
                FlashcardsDueToday = dueToday,
                FlashcardsStudiedToday = studiedTodayCount,
                ActiveAssignments = activeAssignments,
                UpcomingModules = upcomingModules,
                LastWeekCriteriaMet = criteriaMet,
                CurrentStreak = streak
            };
        }
    }
}