using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;

namespace inzBackend.Services.SentenceServices
{
    public interface ISentenceService
    {
        List<SentenceStockDto> GetAllStock();
        void CreateStock(CreateSentenceStockRequest request);
        void DeleteStock(int id);
        Task<int> UploadStockFromExcel(IFormFile file);
        List<SentenceSetGroupDto> GetAllSetsGrouped();
        SentenceSetDto GetSet(int id);
        SentenceSetDto CreateSet(CreateSentenceSetRequest request);
        void DeleteSet(int id);
        void AssignToUser(AssignSentenceRequest request);
        void AssignToModule(AssignSetToModuleRequest request);
        List<SentenceSetDto> GetSetsForModule(int moduleId);
        void RemoveSetFromModule(int moduleId, int setId);
        void UpdateStock(int id, UpdateSentenceStockRequest request);
        Task<List<SearchSentenceResultDto>> SearchSentence(string query, int studentId);
    }
}
