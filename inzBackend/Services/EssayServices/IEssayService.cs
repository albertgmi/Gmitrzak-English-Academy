using inzBackend.Models.EssayModels;

namespace inzBackend.Services.EssayServices
{
    public interface IEssayService
    {
        EssayModuleDto GetEssayModule(int moduleId);
        UserEssayDto SubmitEssay(SubmitEssayRequest request);
        List<UserEssayDto> GetAllEssaysForAdmin();
        List<UserEssayDto> GetEssaysForStudent(int studentId);
        UserEssayDto ReviewEssay(int essayId, ReviewEssayRequest request);
        byte[] ExportEssayToDocx(int essayId);
    }
}