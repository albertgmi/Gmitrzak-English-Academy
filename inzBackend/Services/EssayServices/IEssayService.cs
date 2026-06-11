using inzBackend.Models.EssayModels;

namespace inzBackend.Services.EssayServices
{
    public interface IEssayService
    {
        EssayModuleDto getEssayModule(int moduleId);
        UserEssayDto submitEssay(SubmitEssayRequest request);
        List<UserEssayDto> getAllEssaysForAdmin();
        List<UserEssayDto> getEssaysForStudent(int studentId);
        UserEssayDto reviewEssay(int essayId, ReviewEssayRequest request);
        byte[] exportEssayToDocx(int essayId);
    }
}