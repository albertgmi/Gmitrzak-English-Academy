using inzBackend.Entities;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using Microsoft.EntityFrameworkCore;

public class LessonPanelService : ILessonPanelService
{
    private readonly GmitrzakEnglishAcademyDbContext _dbContext;

    public LessonPanelService(GmitrzakEnglishAcademyDbContext dbContext)
    {
        _dbContext = dbContext;
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
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysFromMonday = ((int)DateTime.UtcNow.DayOfWeek + 6) % 7;
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
            PointDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Points = points,
            Reason = reason
        });
        _dbContext.SaveChanges();
    }

    public LessonFlashcardSummaryDto getFlashcardSummary(int studentUserId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

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

    public List<StreamEntryDto> getStreamEntries(int studentUserId)
    {
        return _dbContext.StreamEntries
            .Where(x => x.UserId == studentUserId)
            .OrderByDescending(x => x.ExecutedAt)
            .Take(100)
            .Select(x => new StreamEntryDto
            {
                Id = x.Id,
                Command = x.Command,
                Payload = x.Payload,
                ExecutedAt = x.ExecutedAt
            })
            .ToList();
    }

    public void addStreamEntry(int studentUserId, string command, string payload)
    {
        _dbContext.StreamEntries.Add(new StreamEntry
        {
            UserId = studentUserId,
            Command = command,
            Payload = payload,
            ExecutedAt = DateTimeOffset.UtcNow
        });
        _dbContext.SaveChanges();
    }

    public void deleteStreamEntry(int entryId)
    {
        var entry = _dbContext.StreamEntries.FirstOrDefault(x => x.Id == entryId);
        if (entry is null) return;
        _dbContext.StreamEntries.Remove(entry);
        _dbContext.SaveChanges();
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
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
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
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
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