using inzBackend.Entities.LearningMaterials;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;

namespace inzBackend.Services.GlobalVocabularyServices
{
    public interface IGlobalVocabularyService
    {
        List<GlobalVocabularyDto> GetAllVocabulary();
        Vocabulary CreateNewVocabulary(VocabularyAddingRequest request);
        void UpdateVocabulary(VocabularyUpdateRequest request, int vocabularyId);
        Task<SearchVocabularyResult> SearchVocabulary(string query, int studentUserId);
        GlobalVocabularyDto AddTranslation(AddTranslationRequest request);
        void AssignVocabularyToStudent(AssignVocabularyToStudentRequest request);
        void AssignMultipleVocabularyToStudent(AssignMultipleVocabularyToStudentRequest request);
    }
}
