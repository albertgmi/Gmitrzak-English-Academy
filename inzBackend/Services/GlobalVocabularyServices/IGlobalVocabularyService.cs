using inzBackend.Entities;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;

namespace inzBackend.Services.GlobalVocabularyServices
{
    public interface IGlobalVocabularyService
    {
        List<VocabularyDto> getAllVocabulary();
        Vocabulary createNewVocabulary(VocabularyAddingRequest request);
        void updateVocabulary(VocabularyUpdateRequest request, int vocabularyId);
    }
}
