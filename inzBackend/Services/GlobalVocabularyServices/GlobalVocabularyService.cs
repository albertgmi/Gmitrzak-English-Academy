using AutoMapper;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Helpers;
using inzBackend.Entities.SpacedRepetition;
using inzBackend.Entities.LearningMaterials;
using inzBackend.Services.AiIntegrationServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.GlobalVocabularyServices
{
    public class GlobalVocabularyService : IGlobalVocabularyService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IAiTranslationService _aiTranslationService;
        private readonly IMapper _mapper;

        public GlobalVocabularyService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper,
            IAiTranslationService aiTranslationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _aiTranslationService = aiTranslationService;
        }

        public List<GlobalVocabularyDto> GetAllVocabulary()
        {
            var vocabularies = _dbContext
                .Vocabulary
                .ToList();

            return _mapper.Map<List<GlobalVocabularyDto>>(vocabularies);
        }

        public Vocabulary CreateNewVocabulary(VocabularyAddingRequest request)
        {
            var vocabulary = new Vocabulary
            {
                Front = request.Front,
                Back = request.Back,
                Category = request.Category
            };

            _dbContext.Vocabulary.Add(vocabulary);
            _dbContext.SaveChanges();

            return vocabulary;
        }

        public void UpdateVocabulary(VocabularyUpdateRequest request, int vocabularyId)
        {
            var vocabulary = _dbContext
                .Vocabulary
                .FirstOrDefault(x => x.Id == vocabularyId);

            if (vocabulary is null)
                throw new NotFoundException($"Vocabulary entry with id: {vocabularyId} was not found");

            vocabulary.Front = request.Front;
            vocabulary.Back = request.Back;
            vocabulary.Category = request.Category;

            _dbContext.SaveChanges();
        }

        public async Task<List<SearchVocabularyResult>> SearchVocabulary(string query, int studentUserId)
        {
            var q = query.ToLower().Trim();

            var vocabMatches = await _dbContext.Vocabulary
                .Where(x => x.Front.ToLower().Contains(q) || x.Back.ToLower().Contains(q))
                .ToListAsync();

            var results = new List<SearchVocabularyResult>();

            if (vocabMatches.Count == 0)
            {
                var translatedList = await _aiTranslationService.TranslateBatchAsync(new List<string> { query });
                var translatedBack = translatedList.FirstOrDefault();

                results.Add(new SearchVocabularyResult
                {
                    Front = query,
                    Back = translatedBack ?? string.Empty,
                    Category = string.Empty,
                    ExistsInGlobal = false,
                    AlreadyAssignedToStudent = false
                });

                return results;
            }

            var assignedVocabIds = (await _dbContext.Flashcards
                .Where(x => x.UserId == studentUserId)
                .Select(x => x.VocabularyId)
                .ToListAsync())
                .ToHashSet();

            foreach (var word in vocabMatches)
            {
                results.Add(new SearchVocabularyResult
                {
                    Id = word.Id,
                    Front = word.Front,
                    Back = word.Back,
                    Category = word.Category,
                    ExistsInGlobal = true,
                    AlreadyAssignedToStudent = assignedVocabIds.Contains(word.Id)
                });
            }

            return results;
        }

        public GlobalVocabularyDto AddTranslation(AddTranslationRequest request)
        {
            var existing = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Front.ToLower() == request.Front.ToLower());

            if (existing is not null)
                return _mapper.Map<GlobalVocabularyDto>(existing);

            var vocab = new Vocabulary
            {
                Front = request.Front,
                Back = request.Back,
                Category = request.Category
            };

            _dbContext.Vocabulary.Add(vocab);
            _dbContext.SaveChanges();

            return _mapper.Map<GlobalVocabularyDto>(vocab);
        }

        public void AssignVocabularyToStudent(AssignVocabularyToStudentRequest request)
        {
            AssignVocabularyIdsToStudent(request.StudentUserId, new List<int> { request.VocabularyId });
        }

        public void AssignMultipleVocabularyToStudent(AssignMultipleVocabularyToStudentRequest request)
        {
            if (request.VocabularyIds == null || !request.VocabularyIds.Any())
                return;

            AssignVocabularyIdsToStudent(request.StudentUserId, request.VocabularyIds);
        }

        public void AssignCatalogueToStudent(AssignCatalogueToStudentRequest request)
        {
            var catalogueExists = _dbContext.Catalogues.Any(x => x.Id == request.CatalogueId);
            if (!catalogueExists)
                throw new NotFoundException("Catalogue not found");

            var vocabularyIds = _dbContext.Vocabulary
                .Where(v => v.CatalogueId == request.CatalogueId)
                .Select(v => v.Id)
                .ToList();

            if (!vocabularyIds.Any())
                throw new BadRequestException("This catalogue has no vocabulary entries to assign.");

            AssignVocabularyIdsToStudent(request.StudentUserId, vocabularyIds);
        }

        private void AssignVocabularyIdsToStudent(int studentUserId, List<int> vocabularyIds)
        {
            var validVocabularyIds = _dbContext.Vocabulary
                .Where(x => vocabularyIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToList();

            if (!validVocabularyIds.Any())
                throw new NotFoundException("None of the specified vocabulary entries were found in global database");

            var alreadyAssignedIds = _dbContext.Flashcards
                .Where(x => x.UserId == studentUserId && validVocabularyIds.Contains(x.VocabularyId))
                .Select(x => x.VocabularyId)
                .ToList();

            var idsToAssign = validVocabularyIds.Except(alreadyAssignedIds).ToList();
            if (!idsToAssign.Any())
                return;

            var today = PolandTime.Today;
            var flashcardsToAdd = idsToAssign.Select(vocabId => new Flashcard
            {
                UserId = studentUserId,
                VocabularyId = vocabId,
                EaseFactor = 250,
                Interval = 0,
                IsLeech = false,
                NextReviewDate = today
            }).ToList();

            _dbContext.Flashcards.AddRange(flashcardsToAdd);
            _dbContext.SaveChanges();
        }
    }
}