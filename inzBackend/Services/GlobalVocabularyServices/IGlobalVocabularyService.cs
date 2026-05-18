using inzBackend.Entities;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;

namespace inzBackend.Services.GlobalVocabularyServices
{
    public interface IGlobalVocabularyService
    {
        List<GlobalVocabularyDto> getAllVocabulary();
        Vocabulary createNewVocabulary(VocabularyAddingRequest request);
        void updateVocabulary(VocabularyUpdateRequest request, int vocabularyId);
        SearchVocabularyResult searchVocabulary(string query, int studentUserId);
        GlobalVocabularyDto addTranslation(AddTranslationRequest request);
        void assignVocabularyToStudent(AssignVocabularyToStudentRequest request);
    }
}
