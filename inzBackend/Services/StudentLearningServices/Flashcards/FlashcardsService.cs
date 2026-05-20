using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Exceptions;

namespace inzBackend.Services.StudentLearningServices.Flashcards
{
    public class FlashcardsService : IFlashcardsService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly ILessonPanelService _lessonPanelService;

        public FlashcardsService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            IMapper mapper, ILessonPanelService lessonPanelService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
            _lessonPanelService = lessonPanelService;
        }

        public List<FlashcardDto> getAllFlashcards()
        {
            var userId = _userContextService.GetUserId;
            var flashcards = _dbContext.Flashcards
                .Include(x => x.Vocabulary)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.NextReviewDate)
                .ToList();
            return _mapper.Map<List<FlashcardDto>>(flashcards);
        }

        public List<FlashcardDto> getLeeches()
        {
            var userId = _userContextService.GetUserId;
            var flashcards = _dbContext.Flashcards
                .Include(x => x.Vocabulary)
                .Where(x => x.UserId == userId && x.IsLeech)
                .OrderBy(x => x.Vocabulary != null ? x.Vocabulary.Front : string.Empty)
                .ToList();
            return _mapper.Map<List<FlashcardDto>>(flashcards);
        }

        public List<FlashcardDto> getStudiedToday()
        {
            var userId = _userContextService.GetUserId;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var studiedIds = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId && x.StudyDate == today)
                .Select(x => x.FlashcardId)
                .Distinct()
                .ToList();

            var flashcards = _dbContext.Flashcards
                .Include(x => x.Vocabulary)
                .Where(x => x.UserId == userId && studiedIds.Contains(x.Id))
                .ToList();

            return _mapper.Map<List<FlashcardDto>>(flashcards);
        }

        public List<FlashcardStudyLogDto> getStudyLogs()
        {
            var userId = _userContextService.GetUserId;
            var studyLogs = _dbContext.FlashcardStudyLogs
                .Include(x => x.Flashcard)
                    .ThenInclude(f => f.Vocabulary)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.StudyDate)
                .ThenBy(x => x.CreatedAt)
                .ToList();

            return _mapper.Map<List<FlashcardStudyLogDto>>(studyLogs);
        }

        public List<FlashcardDto> searchFlashcards(string query)
        {
            var userId = _userContextService.GetUserId;
            var q = query.ToLower();

            var flashcards = _dbContext.Flashcards
                .Include(x => x.Vocabulary)
                .Where(x => x.UserId == userId && x.Vocabulary != null &&
                    (x.Vocabulary.Front.ToLower().Contains(q) ||
                     x.Vocabulary.Back.ToLower().Contains(q)))
                .ToList();

            return _mapper.Map<List<FlashcardDto>>(flashcards);
        }

        public void reviewCard(int flashcardId, ReviewCardRequest request)
        {
            var userId = _userContextService.GetUserId;

            var card = _dbContext.Flashcards
                .FirstOrDefault(x => x.Id == flashcardId && x.UserId == userId);

            if (card is null)
                return;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            switch (request.Quality.ToLower())
            {
                case "easy":
                    card.Interval = card.Interval == 0 ? 2 : card.Interval * 2;
                    card.NextReviewDate = today.AddDays(card.Interval);
                    card.EaseFactor = Math.Min(card.EaseFactor + 10, 300);
                    _lessonPanelService.addActivityPoints((int)userId, 3, "Flashcard done on easy level");
                    break;

                case "hard":
                    card.Interval = 1;
                    card.NextReviewDate = today.AddDays(1);
                    card.EaseFactor = Math.Max(card.EaseFactor - 15, 130);
                    _lessonPanelService.addActivityPoints((int)userId, 2, "Flashcard done on hard level");
                    break;

                case "incorrect":
                    card.Interval = 0;
                    card.NextReviewDate = today;
                    card.EaseFactor = Math.Max(card.EaseFactor - 20, 130);
                    _lessonPanelService.addActivityPoints((int)userId, 1, "Flashcard incorrect, points for trying");
                    break;
            }

            card.IsLeech = card.EaseFactor <= 150;

            var log = _dbContext.FlashcardStudyLogs
                .FirstOrDefault(x => x.UserId == userId
                     && x.StudyDate == today
                     && x.FlashcardId == card.Id);

            if (log is null)
            {
                log = new FlashcardStudyLog
                {
                    UserId = userId!.Value,
                    FlashcardId = card.Id,
                    StudyDate = today,
                    EasyCount = 0,
                    HardCount = 0,
                    IncorrectCount = 0,
                    TimeSpentSeconds = 0
                };
                _dbContext.FlashcardStudyLogs.Add(log);
            }

            switch (request.Quality.ToLower())
            {
                case "easy": log.EasyCount++; break;
                case "hard": log.HardCount++; break;
                case "incorrect": log.IncorrectCount++; break;
            }

            log.TimeSpentSeconds += request.TimeSpentSeconds;

            if (request.Quality.ToLower() == "easy" || request.Quality.ToLower() == "hard")
            {
                bool alreadyGotBonusToday = _dbContext.ActivityPoints
                    .Any(x => x.UserId == userId && x.PointDate == today && x.Reason.Contains("Daily Flashcard Session Completed"));

                if (!alreadyGotBonusToday)
                {
                    bool hasMoreDueCards = _dbContext.Flashcards
                        .Any(x => x.UserId == userId && x.NextReviewDate <= today && x.Id != card.Id);

                    if (!hasMoreDueCards)
                    {
                        _lessonPanelService.addActivityPoints(userId.Value, 15, "Daily Flashcard Session Completed - Bonus!");
                    }
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
