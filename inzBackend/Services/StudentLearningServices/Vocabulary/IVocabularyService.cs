using inzBackend.Models.StudentLearningModels.VocabularyModels;

namespace inzBackend.Services.StudentLearningServices.Vocabulary
{
    public interface IVocabularyService
    {
        List<VocabularyDto> getAllVocabulary();
    }
}