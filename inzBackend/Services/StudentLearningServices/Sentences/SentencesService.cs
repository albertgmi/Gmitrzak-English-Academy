using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using inzBackend.Exceptions;
using inzBackend.Helpers;
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

            var allSentences = _dbContext.Sentences
                .Where(x => x.UserId == userId)
                .Select(x => new SentenceDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    Translation = x.Translation,
                    Notes = x.Notes,
                    IsReviewed = x.IsReviewed,
                    IsPrivate = true,
                    EaseFactor = x.EaseFactor,
                    Interval = x.Interval,
                    IsLeech = x.IsLeech,
                    NextReviewDate = x.NextReviewDate
                }).ToList();

            return allSentences;
        }

        public ModuleSentenceSessionDto getModuleSentences(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;

            var directAssignment = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            var moduleName = directAssignment?.Module.Name ?? "Module";

            var setIds = _dbContext.ModuleSentenceSets
                .Where(x => x.ModuleId == moduleId)
                .Select(x => x.SentenceSetId)
                .ToList();

            var sentences = _dbContext.SentenceSetItems
                .Include(x => x.SentenceStock)
                .Where(x => setIds.Contains(x.SentenceSetId))
                .OrderBy(x => x.SentenceSetId).ThenBy(x => x.Order)
                .ToList();

            var existingAnswers = _dbContext.UserSentenceAnswers
                .Where(x => x.UserId == userId && x.ModuleId == moduleId)
                .ToDictionary(x => x.SentenceStockId);

            return new ModuleSentenceSessionDto
            {
                ModuleId = moduleId,
                ModuleName = moduleName,
                Sentences = sentences.Select(x =>
                {
                    existingAnswers.TryGetValue(x.SentenceStockId, out var prev);
                    return new ModuleSentenceItemDto
                    {
                        SentenceStockId = x.SentenceStockId,
                        Polish = x.SentenceStock.Polish,
                        Order = x.Order,
                        PreviousAnswer = prev?.UserAnswer,
                        PreviousResult = prev != null
                            ? (prev.TeacherOverride ?? prev.AiResult)
                            : null,
                        PreviousExplanation = prev?.AiExplanation,
                        PreviousAnswerId = prev?.Id
                    };
                }).ToList()
            };
        }

        public void reviewSentence(int id, ReviewSentenceRequest request)
        {
            var userId = _userContextService.GetUserId;

            var sentence = _dbContext.Sentences
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);

            if (sentence is null) return;

            var today = PolandTime.Today;

            switch (request.Quality.ToLower())
            {
                case "easy":
                    sentence.Interval = sentence.Interval == 0 ? 2 : sentence.Interval * 2;
                    sentence.NextReviewDate = today.AddDays(sentence.Interval);
                    sentence.EaseFactor = Math.Min(sentence.EaseFactor + 10, 300);
                    break;

                case "hard":
                    sentence.Interval = 1;
                    sentence.NextReviewDate = today.AddDays(1);
                    sentence.EaseFactor = Math.Max(sentence.EaseFactor - 15, 130);
                    break;

                case "incorrect":
                    sentence.Interval = 0;
                    sentence.NextReviewDate = today;
                    sentence.EaseFactor = Math.Max(sentence.EaseFactor - 20, 130);
                    break;
            }

            sentence.IsReviewed = true;
            sentence.IsLeech = sentence.EaseFactor <= 150;

            _dbContext.SaveChanges();
        }

        public List<SentenceDto> getOtherSentences()
        {
            var userId = _userContextService.GetUserId!.Value;

            var sentencesFromSetsQuery = _dbContext.SentenceSetItems
                .Where(item => _dbContext.UserSentenceAssignments
                    .Any(ua => ua.UserId == userId && ua.SentenceSetId == item.SentenceSetId))
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

            var otherSentences = sentencesFromSetsQuery
                .Concat(singleAssignedSentencesQuery)
                .Distinct()
                .ToList();

            return otherSentences;
        }
    }
}