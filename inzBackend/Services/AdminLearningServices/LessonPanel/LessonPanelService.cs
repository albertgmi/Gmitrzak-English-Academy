using AutoMapper;
using ClosedXML.Excel;
using inzBackend.Entities.Administration;
using inzBackend.Entities.Gamification;
using inzBackend.Entities.SpacedRepetition;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AttendanceModels;
using inzBackend.Models.CreditModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

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

    public AgendaDto GetAgenda(int studentUserId)
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

    public void UpdateAgenda(int studentUserId, UpdateAgendaRequest request)
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

    public List<LessonGradeDto> GetGrades(int studentUserId)
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

    public ActivityPointsLessonSummaryDto GetActivityPoints(int studentUserId)
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

    public void AddActivityPoints(int studentUserId, int points, string reason)
    {
        var user = _dbContext.Users
            .FirstOrDefault(u => u.Id == studentUserId)
            ?? throw new NotFoundException($"User with id: {studentUserId} was not found");

        var today = PolandTime.Today;
        var hasActiveBoost = user.DoublePointsExpiresAt.HasValue && user.DoublePointsExpiresAt.Value >= today;

        var finalPoints = hasActiveBoost ? points * 2 : points;
        var finalReason = hasActiveBoost ? $"{reason} (2x boost)" : reason;

        _dbContext.ActivityPoints.Add(new ActivityPoint
        {
            UserId = studentUserId,
            PointDate = today,
            Points = finalPoints,
            Reason = finalReason
        });
        _dbContext.SaveChanges();
    }

    public LessonFlashcardSummaryDto GetFlashcardSummary(int studentUserId)
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

    public List<FlashcardDto> GetAllFlashcardsForUser(int userId)
    {
        var flashcards = _dbContext.Flashcards
            .Include(x => x.Vocabulary)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.NextReviewDate)
            .ToList();
        return _mapper.Map<List<FlashcardDto>>(flashcards);
    }

    public byte[] ExportFlashcardsToPdf(int userId)
    {
        var flashcards = GetAllFlashcardsForUser(userId);
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId)
            ?? throw new NotFoundException($"User with id: {userId} was not found");

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text($"Flashcards - {user.Username}")
                        .FontSize(18).Bold();
                    col.Item().Text($"Generated: {PolandTime.Now:yyyy-MM-dd HH:mm}")
                        .FontSize(9).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(80);
                        columns.ConstantColumn(50);
                    });

                    table.Header(header =>
                    {
                        void HeaderCell(string text) => header.Cell()
                            .Background(Colors.Blue.Darken1)
                            .Padding(5)
                            .Text(text).FontColor(Colors.White).Bold();

                        HeaderCell("No.");
                        HeaderCell("Front");
                        HeaderCell("Back");
                        HeaderCell("Category");
                        HeaderCell("Ease");
                        HeaderCell("Interval");
                        HeaderCell("Next review");
                        HeaderCell("Leech");
                    });

                    int i = 1;
                    foreach (var f in flashcards)
                    {
                        var bg = i % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                        table.Cell().Background(bg).Padding(5).Text(i.ToString());
                        table.Cell().Background(bg).Padding(5).Text(f.Front);
                        table.Cell().Background(bg).Padding(5).Text(f.Back);
                        table.Cell().Background(bg).Padding(5).Text(f.Category);
                        table.Cell().Background(bg).Padding(5).Text(f.EaseFactor.ToString());
                        table.Cell().Background(bg).Padding(5).Text(f.Interval.ToString());
                        table.Cell().Background(bg).Padding(5).Text(f.NextReviewDate.ToString("yyyy-MM-dd"));
                        table.Cell().Background(bg).Padding(5).Text(f.IsLeech ? "YES" : "");

                        i++;
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] ExportFlashcardsToExcel(int userId)
    {
        var flashcards = GetAllFlashcardsForUser(userId);

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Flashcards");

        string[] headers = { "No.", "Front", "Back", "Category", "Ease Factor", "Interval", "Next Review", "Leech" };
        for (int c = 0; c < headers.Length; c++)
        {
            var cell = sheet.Cell(1, c + 1);
            cell.Value = headers[c];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
        }

        int row = 2;
        int rowNumber = 1;
        foreach (var f in flashcards)
        {
            sheet.Cell(row, 1).Value = rowNumber++;
            sheet.Cell(row, 2).Value = f.Front;
            sheet.Cell(row, 3).Value = f.Back;
            sheet.Cell(row, 4).Value = f.Category;
            sheet.Cell(row, 5).Value = f.EaseFactor;
            sheet.Cell(row, 6).Value = f.Interval;
            sheet.Cell(row, 7).Value = f.NextReviewDate.ToString("yyyy-MM-dd");
            sheet.Cell(row, 8).Value = f.IsLeech ? "YES" : "";
            row++;
        }

        sheet.Columns().AdjustToContents();
        sheet.SheetView.FreezeRows(1);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public StudentStudyTimeDto GetStudyTime(int studentUserId)
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

    public LessonLastWeekDto GetLastWeek(int studentUserId)
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

    public LessonStatsDto GetStats(int studentUserId)
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

    public List<AttendanceDto> GetAttendance(int studentId)
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

    public List<AttendanceDto> GetAttendanceHistory(int studentId)
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

    public AttendanceDto AddAttendance(CreateAttendanceDto dto)
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

    public bool DeleteAttendance(int id)
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
    public void UpdateFlashcardInterval(int studentUserId, int flashcardId, int newInterval)
    {
        var card = _dbContext.Flashcards
            .FirstOrDefault(x => x.Id == flashcardId && x.UserId == studentUserId);

        if (card == null)
            throw new NotFoundException("Flashcard not found for this student");

        card.Interval = newInterval;

        _dbContext.SaveChanges();
    }

    public ActivityScoreDto CalculateActivityScore(int studentUserId, DateOnly weekStart, DateOnly weekEnd)
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