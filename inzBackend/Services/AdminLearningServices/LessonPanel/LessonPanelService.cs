using inzBackend.Entities.LearningMaterials;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using Microsoft.EntityFrameworkCore;
using inzBackend.Helpers;
using DocumentFormat.OpenXml.InkML;
using inzBackend.Enums;
using inzBackend.Models.AttendanceModels;
using inzBackend.Exceptions;
using inzBackend.Models.StudentLearningModels.FlashcardModels;
using AutoMapper;
using inzBackend.Services.UserServices;
using inzBackend.Models.CreditModels;
using inzBackend.Entities.SpacedRepetition;
using inzBackend.Entities.Administration;
using inzBackend.Entities.Gamification;

public class LessonPanelService : ILessonPanelService
{
    private readonly GmitrzakEnglishAcademyDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public LessonPanelService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }

    public AgendaDto getAgenda(int studentUserId)
    {
        var agenda = _dbContext.Agendas
            .FirstOrDefault(x => x.UserId == studentUserId);

        if (agenda is null)
        {
            agenda = new Agenda { UserId = studentUserId };
            _dbContext.Agendas.Add(agenda);
            _dbContext.SaveChanges();
        }

        return new AgendaDto
        {
            Id = agenda.Id,
            ActivityPointTarget = agenda.ActivityPointTarget,
            FlashcardTarget = agenda.FlashcardTarget,
            ListeningEpisodeTarget = agenda.ListeningEpisodeTarget,
            Notes = agenda.Notes
        };
    }

    public void updateAgenda(int studentUserId, UpdateAgendaRequest request)
    {
        var agenda = _dbContext.Agendas
            .FirstOrDefault(x => x.UserId == studentUserId);

        if (agenda is null)
        {
            agenda = new Agenda { UserId = studentUserId };
            _dbContext.Agendas.Add(agenda);
        }

        agenda.ActivityPointTarget = request.ActivityPointTarget;
        agenda.FlashcardTarget = request.FlashcardTarget;
        agenda.ListeningEpisodeTarget = request.ListeningEpisodeTarget;
        agenda.Notes = request.Notes;

        _dbContext.SaveChanges();
    }

    public List<LessonGradeDto> getGrades(int studentUserId)
    {
        return _dbContext.Grades
            .Where(x => x.UserId == studentUserId)
            .OrderByDescending(x => x.GradeDate)
            .Select(x => new LessonGradeDto
            {
                Id = x.Id,
                GradeDate = x.GradeDate,
                Percentage = x.Percentage,
                Category = x.Category,
                Notes = x.Notes
            })
            .ToList();
    }

    public ActivityPointsLessonSummaryDto getActivityPoints(int studentUserId)
    {
        var today = PolandTime.Today;
        var daysFromMonday = ((int)PolandTime.DateTimeNow.DayOfWeek + 6) % 7;
        var thisWeekStart = today.AddDays(-daysFromMonday);
        var lastWeekStart = thisWeekStart.AddDays(-7);
        var lastWeekEnd = thisWeekStart.AddDays(-1);

        var all = _dbContext.ActivityPoints
            .Where(x => x.UserId == studentUserId)
            .OrderByDescending(x => x.PointDate)
            .ToList();

        return new ActivityPointsLessonSummaryDto
        {
            TotalAllTime = all.Sum(x => x.Points),
            TotalThisWeek = all.Where(x => x.PointDate >= thisWeekStart).Sum(x => x.Points),
            TotalLastWeek = all.Where(x => x.PointDate >= lastWeekStart
                                        && x.PointDate <= lastWeekEnd).Sum(x => x.Points),
            History = all.Select(x => new ActivityPointLessonDto
            {
                Id = x.Id,
                PointDate = x.PointDate,
                Points = x.Points,
                Reason = x.Reason
            }).ToList()
        };
    }

    public void addActivityPoints(int studentUserId, int points, string reason)
    {
        _dbContext.ActivityPoints.Add(new ActivityPoint
        {
            UserId = studentUserId,
            PointDate = PolandTime.Today,
            Points = points,
            Reason = reason
        });
        _dbContext.SaveChanges();
    }

    public LessonFlashcardSummaryDto getFlashcardSummary(int studentUserId)
    {
        var today = PolandTime.Today;

        var allCards = _dbContext.Flashcards
            .Include(x => x.Vocabulary)
            .Where(x => x.UserId == studentUserId)
            .ToList();

        var studiedTodayIds = _dbContext.FlashcardStudyLogs
            .Where(x => x.UserId == studentUserId && x.StudyDate == today)
            .Select(x => x.FlashcardId)
            .Distinct()
            .ToList();

        var recentLogs = _dbContext.FlashcardStudyLogs
            .Where(x => x.UserId == studentUserId)
            .GroupBy(x => x.StudyDate)
            .OrderByDescending(g => g.Key)
            .Take(14)
            .Select(g => new LessonStudyLogDto
            {
                StudyDate = g.Key,
                EasyCount = g.Sum(x => x.EasyCount),
                HardCount = g.Sum(x => x.HardCount),
                IncorrectCount = g.Sum(x => x.IncorrectCount),
                TimeSpentSeconds = g.Sum(x => x.TimeSpentSeconds)
            })
            .ToList();

        return new LessonFlashcardSummaryDto
        {
            TotalCards = allCards.Count,
            LeechCount = allCards.Count(x => x.IsLeech),
            StudiedTodayCount = studiedTodayIds.Count,
            DueCount = allCards.Count(x => x.NextReviewDate <= today),
            Leeches = allCards.Where(x => x.IsLeech)
                .Select(MapFlashcard).ToList(),
            StudiedToday = allCards.Where(x => studiedTodayIds.Contains(x.Id))
                .Select(MapFlashcard).ToList(),
            RecentLogs = recentLogs
        };
    }

    public List<FlashcardDto> getAllFlashcardsForUser(int userId)
    {
        var flashcards = _dbContext.Flashcards
            .Include(x => x.Vocabulary)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.NextReviewDate)
            .ToList();
        return _mapper.Map<List<FlashcardDto>>(flashcards);
    }

    public StudentStudyTimeDto getStudyTime(int studentUserId)
    {
        var logs = _dbContext.FlashcardStudyLogs
            .Where(x => x.UserId == studentUserId)
            .ToList();

        var daily = logs
            .GroupBy(x => x.StudyDate)
            .OrderByDescending(g => g.Key)
            .Take(30)
            .Select(g => new DailyStudyTimeDto
            {
                StudyDate = g.Key,
                TimeSpentSeconds = g.Sum(x => x.TimeSpentSeconds),
                FlashcardsDone = g.Sum(x => x.EasyCount + x.HardCount + x.IncorrectCount),
                EasyCount = g.Sum(x => x.EasyCount),
                HardCount = g.Sum(x => x.HardCount),
                IncorrectCount = g.Sum(x => x.IncorrectCount)
            })
            .ToList();

        return new StudentStudyTimeDto
        {
            TotalTimeSpentSeconds = logs.Sum(x => x.TimeSpentSeconds),
            TotalFlashcardsDone = logs.Sum(x => x.EasyCount + x.HardCount + x.IncorrectCount),
            EasyCount = logs.Sum(x => x.EasyCount),
            HardCount = logs.Sum(x => x.HardCount),
            IncorrectCount = logs.Sum(x => x.IncorrectCount),
            DailyBreakdown = daily
        };
    }

    public LessonLastWeekDto getLastWeek(int studentUserId)
    {
        var today = PolandTime.Today;
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-daysFromMonday);
        var weekEnd = weekStart.AddDays(6);

        var agenda = _dbContext.Agendas
            .FirstOrDefault(x => x.UserId == studentUserId);

        var activityPoints = _dbContext.ActivityPoints
            .Where(x => x.UserId == studentUserId
                     && x.PointDate >= weekStart
                     && x.PointDate <= weekEnd)
            .Sum(x => (int?)x.Points) ?? 0;

        var flashcardLogs = _dbContext.FlashcardStudyLogs
            .Where(x => x.UserId == studentUserId
                     && x.StudyDate >= weekStart
                     && x.StudyDate <= weekEnd)
            .ToList();

        var flashcardsStudied = flashcardLogs.Sum(x => x.EasyCount + x.HardCount + x.IncorrectCount);
        var flashcardTimeSeconds = flashcardLogs.Sum(x => x.TimeSpentSeconds);

        var listeningEpisodes = _dbContext.ListeningReports
            .Where(x => x.UserId == studentUserId
                     && x.ReportDate >= weekStart
                     && x.ReportDate <= weekEnd)
            .Sum(x => (int?)x.EpisodeCount) ?? 0;

        var grades = _dbContext.Grades
            .Where(x => x.UserId == studentUserId
                     && x.GradeDate >= weekStart
                     && x.GradeDate <= weekEnd)
            .Select(x => new LessonGradeDto
            {
                Id = x.Id,
                GradeDate = x.GradeDate,
                Percentage = x.Percentage,
                Category = x.Category,
                Notes = x.Notes
            })
            .ToList();

        var ptTarget = agenda?.ActivityPointTarget ?? 500;
        var fcTarget = agenda?.FlashcardTarget ?? 50;
        var lisTarget = agenda?.ListeningEpisodeTarget ?? 1;

        return new LessonLastWeekDto
        {
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            TotalActivityPoints = activityPoints,
            FlashcardsStudied = flashcardsStudied,
            FlashcardTimeSeconds = flashcardTimeSeconds,
            ListeningEpisodesWatched = listeningEpisodes,
            GradesThisWeek = grades,
            RankingCriteriaMet = activityPoints >= ptTarget
                                    && flashcardsStudied >= fcTarget
                                    && listeningEpisodes >= lisTarget,
            ActivityPointTarget = ptTarget,
            FlashcardTarget = fcTarget,
            ListeningEpisodeTarget = lisTarget
        };
    }

    public LessonStatsDto getStats(int studentUserId)
    {
        var today = PolandTime.Today;
        var last30Days = today.AddDays(-30);

        var dailyActivity = _dbContext.ActivityPoints
            .Where(x => x.UserId == studentUserId && x.PointDate >= last30Days)
            .GroupBy(x => x.PointDate)
            .Select(g => new LessonDailyActivityDto
            {
                Date = g.Key,
                Points = g.Sum(x => x.Points)
            })
            .OrderBy(x => x.Date)
            .ToList();

        var dailyFlashcards = _dbContext.FlashcardStudyLogs
            .Where(x => x.UserId == studentUserId && x.StudyDate >= last30Days)
            .GroupBy(x => x.StudyDate)
            .Select(g => new LessonDailyFlashcardsDto
            {
                Date = g.Key,
                CardsStudied = g.Sum(x => x.EasyCount + x.HardCount + x.IncorrectCount),
                TimeSpentSeconds = g.Sum(x => x.TimeSpentSeconds)
            })
            .OrderBy(x => x.Date)
            .ToList();

        var grades = _dbContext.Grades
            .Where(x => x.UserId == studentUserId)
            .OrderByDescending(x => x.GradeDate)
            .Select(x => new LessonGradeDto
            {
                Id = x.Id,
                GradeDate = x.GradeDate,
                Percentage = x.Percentage,
                Category = x.Category,
                Notes = x.Notes
            })
            .ToList();

        return new LessonStatsDto
        {
            DailyActivity = dailyActivity,
            DailyFlashcards = dailyFlashcards,
            GradeHistory = grades,
            CategoryBreakdown = new LessonCategoryBreakdownDto
            {
                AvgVocabulary = grades.Where(x => x.Category == "Vocabulary").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
                AvgSentences = grades.Where(x => x.Category == "Sentences").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
                AvgMemories = grades.Where(x => x.Category == "Memories").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
                AvgPronunciation = grades.Where(x => x.Category == "Pronunciation").Select(x => x.Percentage).DefaultIfEmpty(0).Average(),
            }
        };
    }

    public List<AttendanceDto> getAttendance(int studentId)
    {
        var now = PolandTime.Now;

        var firstDayOfMonth = new DateTimeOffset(
            now.Year,
            now.Month,
            1,
            0,
            0,
            0,
            now.Offset
        );

        var records = _dbContext.Attendance
            .Where(a =>
                a.UserId == studentId &&
                a.CreatedAt >= firstDayOfMonth)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Type = a.Type.ToString(),
                Duration = a.DurationInMinutes,
                CreatedAt = PolandTime.Convert(a.CreatedAt).DateTime
            })
            .ToList();

        return records;
    }

    public List<AttendanceDto> getAttendanceHistory(int studentId)
    {
        var now = PolandTime.Now;

        var firstDayOfMonth = new DateTimeOffset(
            now.Year,
            now.Month,
            1,
            0,
            0,
            0,
            0,
            now.Offset
        );

        var records = _dbContext.Attendance
            .Where(a =>
                a.UserId == studentId &&
                a.CreatedAt < firstDayOfMonth)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Type = a.Type.ToString(),
                Duration = a.DurationInMinutes,
                CreatedAt = PolandTime.Convert(a.CreatedAt).DateTime
            })
            .ToList();

        return records;
    }

    public AttendanceDto addAttendance(CreateAttendanceDto dto)
    {
        var studentExists = _dbContext
            .Users
            .Any(x => x.Id == dto.UserId);

        if (!studentExists)
            throw new Exception($"Student with ID {dto.UserId} not found.");

        if (!Enum.TryParse<AttendanceType>(dto.Type, true, out var attendanceType))
            throw new Exception("Invalid attendance type. Use 'SCHEDULED' or 'MAKEUP'.");

        var attendance = new Attendance
        {
            UserId = dto.UserId,
            Type = attendanceType,
            DurationInMinutes = dto.Duration,
            CreatedAt = PolandTime.Now
        };

        _dbContext.Attendance.Add(attendance);
        _dbContext.SaveChanges();

        return new AttendanceDto
        {
            Id = attendance.Id,
            UserId = attendance.UserId,
            Type = attendance.Type.ToString(),
            Duration = attendance.DurationInMinutes,
            CreatedAt = PolandTime.Now.DateTime
        };
    }

    public bool deleteAttendance(int id)
    {
        var attendance = _dbContext
            .Attendance
            .FirstOrDefault(a => a.Id == id);

        if (attendance is null)
            throw new NotFoundException("Attendance not found");

        _dbContext.Attendance.Remove(attendance);
        _dbContext.SaveChanges();

        return true;
    }
    public void updateFlashcardInterval(int studentUserId, int flashcardId, int newInterval)
    {
        var card = _dbContext.Flashcards
            .FirstOrDefault(x => x.Id == flashcardId && x.UserId == studentUserId);

        if (card == null)
            throw new NotFoundException("Flashcard not found for this student");

        card.Interval = newInterval;

        _dbContext.SaveChanges();
    }

    public ActivityScoreDto calculateActivityScore(int studentUserId, DateOnly weekStart, DateOnly weekEnd)
    {
        var dueModules = _dbContext.UserModuleAssignments
            .Where(x => x.UserId == studentUserId
                     && x.DueDate >= weekStart
                     && x.DueDate <= weekEnd)
            .ToList();

        double homeworkScore = 0;
        if (dueModules.Any())
        {
            var onTime = dueModules.Count(x => x.IsCompleted);
            homeworkScore = (double)onTime / dueModules.Count * 100;
        }

        var startDate = weekStart.ToDateTime(TimeOnly.MinValue);
        var endDate = weekEnd.ToDateTime(TimeOnly.MaxValue);

        var attendanceCount = _dbContext.Attendance
            .Where(x => x.UserId == studentUserId
                     && x.CreatedAt >= startDate
                     && x.CreatedAt <= endDate)
            .Count();

        double attendanceScore = Math.Min(100, attendanceCount / 2.0 * 100);

        var watchingDone = _dbContext.UserModuleAssignments
            .Where(x => x.UserId == studentUserId
                     && x.IsCompleted
                     && x.Module.Category == "Watching"
                     && x.DueDate >= weekStart
                     && x.DueDate <= weekEnd)
            .Count();

        var watchingFromMatrix = _dbContext.UserMatrixModuleCompletions
            .Where(x => x.UserId == studentUserId
                     && x.CompletedDate >= weekStart
                     && x.CompletedDate <= weekEnd
                     && x.MatrixModule.Module.Category == "Watching")
            .Count();

        double watchingScore = (watchingDone + watchingFromMatrix) > 0 ? 100 : 0;

        var flashcardDays = _dbContext.FlashcardStudyLogs
            .Where(x => x.UserId == studentUserId
                     && x.StudyDate >= weekStart
                     && x.StudyDate <= weekEnd)
            .Select(x => x.StudyDate)
            .Distinct()
            .Count();

        double regularityScore = Math.Min(100, flashcardDays / 3.0 * 100);

        var agenda = _dbContext.Agendas.FirstOrDefault(x => x.UserId == studentUserId);
        var fcTarget = agenda?.FlashcardTarget ?? 50;

        var fcDone = _dbContext.FlashcardStudyLogs
            .Where(x => x.UserId == studentUserId
                     && x.StudyDate >= weekStart
                     && x.StudyDate <= weekEnd)
            .Sum(x => (int?)(x.EasyCount + x.HardCount + x.IncorrectCount)) ?? 0;

        double flashcardScore = fcTarget > 0
            ? Math.Min(100, (double)fcDone / fcTarget * 100)
            : 0;

        var activityPointsSum = _dbContext.ActivityPoints
            .Where(x => x.UserId == studentUserId
                     && x.PointDate >= weekStart
                     && x.PointDate <= weekEnd)
            .Sum(x => (int?)x.Points) ?? 0;

        var criteriaScore = (int)Math.Round(
            homeworkScore * 0.30 +
            attendanceScore * 0.20 +
            watchingScore * 0.15 +
            regularityScore * 0.20 +
            flashcardScore * 0.15
        );

        return new ActivityScoreDto
        {
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            TotalScore = criteriaScore + activityPointsSum,
            HomeworkScore = (int)Math.Round(homeworkScore),
            AttendanceScore = (int)Math.Round(attendanceScore),
            WatchingScore = (int)Math.Round(watchingScore),
            RegularityScore = (int)Math.Round(regularityScore),
            FlashcardScore = (int)Math.Round(flashcardScore),
            HomeworkDone = dueModules.Count(x => x.IsCompleted),
            HomeworkTotal = dueModules.Count,
            AttendanceCount = attendanceCount,
            FlashcardDays = flashcardDays,
            FlashcardsDone = fcDone,
            FlashcardTarget = fcTarget
        };
    }

    private static LessonFlashcardDto MapFlashcard(Flashcard x)
    {
        return new LessonFlashcardDto
        {
            Id = x.Id,
            Front = x.Vocabulary != null ? x.Vocabulary.Front : string.Empty,
            Back = x.Vocabulary != null ? x.Vocabulary.Back : string.Empty,
            Category = x.Vocabulary != null ? x.Vocabulary.Category : string.Empty,
            Interval = x.Interval,
            IsLeech = x.IsLeech,
            NextReviewDate = x.NextReviewDate
        };
    }
}