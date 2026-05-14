using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using AutoMapper;

namespace inzBackend.Services.StudentLearningServices.Flashcards
{
    public class FlashcardsService : IFlashcardsService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public FlashcardsService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public List<FlashcardDto> getAllFlashcards()
        {
            var userId = _userContextService.GetUserId;
            var flashcards = _dbContext.Flashcards
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.NextReviewDate)
                .ToList();
            return _mapper.Map<List<FlashcardDto>>(flashcards);
        }

        public List<FlashcardDto> getLeeches()
        {
            var userId = _userContextService.GetUserId;
            var flashcards = _dbContext
                .Flashcards
                .Where(x => x.UserId == userId && x.IsLeech)
                .OrderBy(x => x.Front)
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

            var flashcards = _dbContext
                .Flashcards
                .Where(x => x.UserId == userId && studiedIds.Contains(x.Id))
                .ToList();

            return _mapper.Map<List<FlashcardDto>>(flashcards);
        }

        public List<FlashcardStudyLogDto> getStudyLogs()
        {
            var userId = _userContextService.GetUserId;
            return _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.StudyDate)
                .Select(x => new FlashcardStudyLogDto
                {
                    Id = x.Id,
                    StudyDate = x.StudyDate,
                    EasyCount = x.EasyCount,
                    HardCount = x.HardCount,
                    IncorrectCount = x.IncorrectCount,
                    TimeSpentSeconds = x.TimeSpentSeconds
                })
                .ToList();
        }

        public List<FlashcardDto> searchFlashcards(string query)
        {
            var userId = _userContextService.GetUserId;
            var q = query.ToLower();
            var flashcards = _dbContext
                .Flashcards
                .Where(x => x.UserId == userId &&
                    (x.Front.ToLower().Contains(q) ||
                     x.Back.ToLower().Contains(q)))
                .ToList();
            return _mapper.Map<List<FlashcardDto>>(flashcards);
        }
    }
}
