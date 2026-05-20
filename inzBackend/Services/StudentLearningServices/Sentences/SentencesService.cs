using AutoMapper;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Models.StudentLearningModels.SentenceModels;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var userId = _userContextService.GetUserId!.Value;

            var privateSentencesQuery = _dbContext.Sentences
                .Where(x => x.UserId == userId)
                .Select(x => new SentenceDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    Translation = x.Translation,
                    Notes = x.Notes,
                    IsReviewed = x.IsReviewed,
                    IsPrivate = true
                });

            var sentencesFromSetsQuery = _dbContext.SentenceSetItems
                .Where(item => _dbContext.UserSentenceAssignments
                    .Where(ua => ua.UserId == userId && ua.SentenceSetId != null)
                    .Select(ua => ua.SentenceSetId)
                    .Contains(item.SentenceSetId))
                .Select(item => new SentenceDto
                {
                    Id = item.SentenceStockId,
                    Content = item.SentenceStock.Polish,
                    Translation = item.SentenceStock.EnglishTranslation,
                    Notes = "Assigned from set: " + item.SentenceSet.Name,
                    IsReviewed = _dbContext.UserSentenceAssignments
                        .Where(ua => ua.UserId == userId && ua.SentenceSetId == item.SentenceSetId)
                        .Select(ua => ua.IsReviewed)
                        .FirstOrDefault(),
                    IsPrivate = false
                });

            var singleAssignedSentencesQuery = _dbContext.UserSentenceAssignments
                .Where(x => x.UserId == userId && x.SentenceStockId != null)
                .Select(x => new SentenceDto
                {
                    Id = x.SentenceStockId!.Value,
                    Content = x.SentenceStock.Polish,
                    Translation = x.SentenceStock.EnglishTranslation,
                    Notes = "Single sentence assigned",
                    IsReviewed = x.IsReviewed,
                    IsPrivate = false
                });

            var allSentences = privateSentencesQuery
                .Concat(sentencesFromSetsQuery)
                .Concat(singleAssignedSentencesQuery)
                .Distinct()
                .ToList();

            return allSentences;
        }

        public ModuleSentenceSessionDto getModuleSentences(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;

            var matrixAssignment = _dbContext.UserMatrixModuleCompletions
                .Include(x => x.MatrixModule).ThenInclude(mm => mm.Module)
                .FirstOrDefault(x => x.UserId == userId
                                  && x.MatrixModule.ModuleId == moduleId);

            var directAssignment = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            var moduleName = directAssignment?.Module.Name
                ?? matrixAssignment?.MatrixModule.Module.Name
                ?? "Module";

            var setIds = _dbContext.ModuleSentenceSets
                .Where(x => x.ModuleId == moduleId)
                .Select(x => x.SentenceSetId)
                .ToList();

            var sentences = _dbContext.SentenceSetItems
                .Include(x => x.SentenceStock)
                .Where(x => setIds.Contains(x.SentenceSetId))
                .OrderBy(x => x.SentenceSetId).ThenBy(x => x.Order)
                .Select(x => new ModuleSentenceItemDto
                {
                    SentenceStockId = x.SentenceStockId,
                    Polish = x.SentenceStock.Polish,
                    Order = x.Order.ToString()
                })
                .ToList();

            return new ModuleSentenceSessionDto
            {
                ModuleId = moduleId,
                ModuleName = moduleName,
                AssignmentId = directAssignment?.Id ?? 0,
                Sentences = sentences
            };
        }

        public void reviewSentence(int id)
        {
            var userId = _userContextService.GetUserId!.Value;

            var privateSentence = _dbContext
                .Sentences
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);
            if (privateSentence != null)
            {
                privateSentence.IsReviewed = true;
            }
            else
            {
                var assignment = _dbContext.UserSentenceAssignments
                    .FirstOrDefault(x => x.UserId == userId && x.SentenceStockId == id);

                if (assignment == null)
                {
                    var setIds = _dbContext.SentenceSetItems
                        .Where(ssi => ssi.SentenceStockId == id)
                        .Select(ssi => ssi.SentenceSetId)
                        .ToList();

                    assignment = _dbContext.UserSentenceAssignments
                        .FirstOrDefault(x => x.UserId == userId && x.SentenceSetId.HasValue && setIds.Contains(x.SentenceSetId.Value));
                }

                if (assignment != null)
                {
                    assignment.IsReviewed = true;
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
