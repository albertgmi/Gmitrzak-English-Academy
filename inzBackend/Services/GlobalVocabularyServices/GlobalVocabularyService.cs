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

        public async Task<SearchVocabularyResult> SearchVocabulary(string query, int studentUserId)
        {
            var q = query.ToLower().Trim();

            var vocab = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Front.ToLower().Contains(q) || x.Back.ToLower().Contains(q));

            List<string> toTranslate = new List<string>();
            toTranslate.Add(query);
            var translatedList = _aiTranslationService
                .TranslateBatchAsync(toTranslate);
            var translatedBack = (await translatedList).FirstOrDefault();

            if (vocab is null)
            {
                return new SearchVocabularyResult
                {
                    Front = query,
                    Back = translatedBack ?? string.Empty,
                    Category = string.Empty,
                    ExistsInGlobal = false,
                    AlreadyAssignedToStudent = false
                };
            }

            var alreadyAssigned = _dbContext.Flashcards
                .Any(x => x.UserId == studentUserId && x.VocabularyId == vocab.Id);

            return new SearchVocabularyResult
            {
                Id = vocab.Id,
                Front = vocab.Front,
                Back = vocab.Back,
                Category = vocab.Category,
                ExistsInGlobal = true,
                AlreadyAssignedToStudent = alreadyAssigned
            };
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
            var vocab = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Id == request.VocabularyId);

            if (vocab is null)
                throw new NotFoundException("Vocabulary entry not found in global database");

            var alreadyExists = _dbContext.Flashcards
                .Any(x => x.UserId == request.StudentUserId && x.VocabularyId == vocab.Id);

            if (alreadyExists) return;

            var today = PolandTime.Today;

            var flashcard = new Flashcard
            {
                UserId = request.StudentUserId,
                VocabularyId = vocab.Id,
                EaseFactor = 250,
                Interval = 0,
                IsLeech = false,
                NextReviewDate = today
            };

            _dbContext.Flashcards.Add(flashcard);
            _dbContext.SaveChanges();
        }

        public void AssignMultipleVocabularyToStudent(AssignMultipleVocabularyToStudentRequest request)
        {
            if (request.VocabularyIds == null || !request.VocabularyIds.Any())
                return;

            var validVocabularyIds = _dbContext.Vocabulary
                .Where(x => request.VocabularyIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToList();

            if (!validVocabularyIds.Any())
                throw new NotFoundException("None of the specified vocabulary entries were found in global database");

            var alreadyAssignedIds = _dbContext.Flashcards
                .Where(x => x.UserId == request.StudentUserId && validVocabularyIds.Contains(x.VocabularyId))
                .Select(x => x.VocabularyId)
                .ToList();

            var idsToAssign = validVocabularyIds.Except(alreadyAssignedIds).ToList();

            if (!idsToAssign.Any())
                return;

            var today = PolandTime.Today;
            var flashcardsToAdd = new List<Flashcard>();

            foreach (var vocabId in idsToAssign)
            {
                flashcardsToAdd.Add(new Flashcard
                {
                    UserId = request.StudentUserId,
                    VocabularyId = vocabId,
                    EaseFactor = 250,
                    Interval = 0,
                    IsLeech = false,
                    NextReviewDate = today
                });
            }

            _dbContext.Flashcards.AddRange(flashcardsToAdd);
            _dbContext.SaveChanges();
        }
    }
}