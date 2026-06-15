using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;

namespace inzBackend.Services.SentenceServices
{
    public interface ISentenceService
    {
        List<SentenceStockDto> getAllStock();
        void createStock(CreateSentenceStockRequest request);
        void deleteStock(int id);
        Task<int> uploadStockFromExcel(IFormFile file);
        List<SentenceSetGroupDto> getAllSetsGrouped();
        SentenceSetDto getSet(int id);
        SentenceSetDto createSet(CreateSentenceSetRequest request);
        void deleteSet(int id);
        void assignToUser(AssignSentenceRequest request);
        void assignToModule(AssignSetToModuleRequest request);
        List<SentenceSetDto> getSetsForModule(int moduleId);
        void removeSetFromModule(int moduleId, int setId);
        void updateStock(int id, UpdateSentenceStockRequest request);
        Task<List<SearchSentenceResultDto>> searchSentence(string query, int studentId);
    }
}
