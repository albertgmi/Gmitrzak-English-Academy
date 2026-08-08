using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Services.AdminLearningServices.Pronunciation
{
    public interface IAdminPronunciationService
    {
        List<AdminPronunciationDto> GetStudentPronunciation(int studentId);
        void UpdatePronunciation(int id, UpdatePronunciationRequest request);
        void DeletePronunciation(int id);
    }
}
