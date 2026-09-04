using inzBackend.Enums;
using inzBackend.Models.AcademyExamModels;

namespace inzBackend.Services.AcademyExamServices
{
    public interface IAcademyExamService
    {
        List<AcademyExamDto> GetExamsByLevel(ExamLevel level, int currentUserId);
        AcademyExamDto GetExamById(int examId, int currentUserId);
        void SignUpForExam(int examId, int currentUserId);
        void UnsignFromExam(int examId, int currentUserId);

        // Admin operations
        int CreateExam(CreateAcademyExamDto dto);
        void UpdateExam(int examId, UpdateAcademyExamDto dto);
        void DeleteExam(int examId);
        void MarkTakerStatus(int examId, int targetUserId, ExamSignupStatus status);
    }
}
