using AutoMapper;
using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Services.GlobalVocabularyServices
{
    public class GlobalVocabularyService : IGlobalVocabularyService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IMapper _mapper;

        public GlobalVocabularyService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<GlobalVocabularyDto> getAllVocabulary()
        {
            var vocabularies = _dbContext
                .Vocabulary
                .ToList();

            return _mapper.Map<List<GlobalVocabularyDto>>(vocabularies);
        }

        public Vocabulary createNewVocabulary(VocabularyAddingRequest request)
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

        public void updateVocabulary(VocabularyUpdateRequest request, int vocabularyId)
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

        public SearchVocabularyResult searchVocabulary(string query, int studentUserId)
        {
            var q = query.ToLower().Trim();

            var vocab = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Front.ToLower().Contains(q) || x.Back.ToLower().Contains(q));

            if (vocab is null)
            {
                return new SearchVocabularyResult
                {
                    Front = query,
                    Back = string.Empty,
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

        public GlobalVocabularyDto addTranslation(AddTranslationRequest request)
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

        public void assignVocabularyToStudent(AssignVocabularyToStudentRequest request)
        {
            var vocab = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Id == request.VocabularyId);

            if (vocab is null)
                throw new NotFoundException("Vocabulary entry not found in global database");

            var alreadyExists = _dbContext.Flashcards
                .Any(x => x.UserId == request.StudentUserId && x.VocabularyId == vocab.Id);

            if (alreadyExists) return;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

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

        public void assignMultipleVocabularyToStudent(AssignMultipleVocabularyToStudentRequest request)
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

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
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