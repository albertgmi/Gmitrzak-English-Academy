using AutoMapper;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.SentenceModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentLearningServices.Sentences
{
    public class SentencesService : ISentencesService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public SentencesService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public List<SentenceDto> getAllSentences()
        {
            var userId = _userContextService.GetUserId;

            var privateSentencesQuery = _dbContext.Sentences
                .Where(x => x.UserId == userId)
                .Select(x => new SentenceDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    Translation = x.Translation,
                    Notes = x.Notes
                });

            var assignedSetIdsQuery = _dbContext.UserSentenceAssignments
                .Where(x => x.UserId == userId && x.SentenceSetId != null)
                .Select(x => x.SentenceSetId);

            var sentencesFromSetsQuery = _dbContext.SentenceSetItems
                .Where(item => assignedSetIdsQuery.Contains(item.SentenceSetId))
                .Select(item => new SentenceDto
                {
                    Id = item.SentenceStockId,
                    Content = item.SentenceStock.Polish,
                    Translation = item.SentenceStock.EnglishTranslation,
                    Notes = "Assigned from set: " + item.SentenceSet.Name
                });

            var singleAssignedSentencesQuery = _dbContext.UserSentenceAssignments
                .Where(x => x.UserId == userId && x.SentenceStockId != null)
                .Select(x => new SentenceDto
                {
                    Id = x.SentenceStockId!.Value,
                    Content = x.SentenceStock.Polish,
                    Translation = x.SentenceStock.EnglishTranslation,
                    Notes = "Single sentence assigned"
                });

            var allSentences = privateSentencesQuery
                .Concat(sentencesFromSetsQuery)
                .Concat(singleAssignedSentencesQuery)
                .Distinct()
                .ToList();

            return allSentences;
        }
    }
}
