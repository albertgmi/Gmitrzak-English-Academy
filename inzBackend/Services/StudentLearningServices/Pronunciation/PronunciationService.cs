using inzBackend.Enums;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentLearningServices.Pronunciation
{
    public class PronunciationService : IPronunciationService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public PronunciationService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }
        public List<PronunciationEntryDto> getAllEntries()
        {
            var userId = _userContextService.GetUserId!.Value;

            return _dbContext.PronunciationEntries
                .Where(x => x.UserId == userId
                         && x.IsInCurrentSession
                         && x.Status != PronunciationStatus.Correct)
                .OrderBy(x => x.Status == PronunciationStatus.Incorrect ? 0 : 1)
                .ThenBy(x => x.SortOrder)
                .Select(x => new PronunciationEntryDto
                {
                    Id = x.Id,
                    Word = x.Word,
                    Status = x.Status.ToString(),
                    SortOrder = x.SortOrder,
                    IsInCurrentSession = x.IsInCurrentSession
                })
                .ToList();
        }

        public List<PronunciationTestItemDto> getCorrectPronunciation()
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var entries = _dbContext.PronunciationEntries
                .Where(x => x.UserId == userId
                         && x.Status == PronunciationStatus.Correct)
                .OrderByDescending(x => x.MarkedCorrectAt)
                .Select(x => new PronunciationTestItemDto
                {
                    Id = x.Id,
                    Word = x.Word,
                    Status = x.Status.ToString(),
                    SortOrder = x.SortOrder,
                    MarkedCorrectAt = x.MarkedCorrectAt,
                    DaysUntilRefresh = x.MarkedCorrectAt.HasValue
                        ? Math.Max(0, 30 - (today.DayNumber - x.MarkedCorrectAt.Value.DayNumber))
                        : null
                })
                .ToList();
            return entries;
        }
    }
}