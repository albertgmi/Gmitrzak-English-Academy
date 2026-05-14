using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;

namespace inzBackend.Services.StudentLearningServices.Vocabulary
{
    public class VocabularyService : IVocabularyService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public VocabularyService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public List<VocabularyDto> getAllVocabulary()
        {
            var userId = _userContextService.GetUserId;
            return _dbContext
                .Flashcards
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Front)
                .Select(x => new VocabularyDto
                {
                    Id = x.Id,
                    Front = x.Front,
                    Back = x.Back,
                    Category = x.Category,
                    Interval = x.Interval,
                    IsLeech = x.IsLeech,
                    NextReviewDate = x.NextReviewDate
                })
                .ToList();
        }
    }
}
