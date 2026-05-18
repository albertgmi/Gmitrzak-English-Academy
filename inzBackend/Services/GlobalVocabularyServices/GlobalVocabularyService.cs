using AutoMapper;
using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using inzBackend.Models.GlobalVocabularyModels;

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

        public List<VocabularyDto> getAllVocabulary()
        {
            var vocabularies = _dbContext
                .Vocabulary
                .ToList();

            return _mapper.Map<List<VocabularyDto>>(vocabularies);
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
    }
}
