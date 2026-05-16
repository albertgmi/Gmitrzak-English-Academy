using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Services.AdminFlashcardServices
{
    public interface IAdminFlashcardService
    {
        List<AdminStudentStudySummaryDto> getAdminStudyLogsSummary();
    }
}