using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

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
                .Include(x => x.Vocabulary)
                .OrderBy(x => x.Vocabulary.Front)
                .Select(x => new VocabularyDto
                {
                    Id = x.Id,
                    Front = x.Vocabulary.Front,
                    Back = x.Vocabulary.Back,
                    Category = x.Vocabulary.Category,
                    Interval = x.Interval,
                    IsLeech = x.IsLeech,
                    NextReviewDate = x.NextReviewDate
                })
                .ToList();
        }
    }
}
