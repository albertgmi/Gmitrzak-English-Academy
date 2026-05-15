using inzBackend.Models.AdminFlashcardModels;

namespace inzBackend.Services.AdminFlashcardServices
{
    public interface IAdminFlashcardService
    {
        List<AdminStudentStudySummaryDto> getAdminStudyLogsSummary();
    }
}