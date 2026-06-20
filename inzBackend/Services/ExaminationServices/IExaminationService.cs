using inzBackend.Models.ExaminationModels;

namespace inzBackend.Services.ExaminationServices
{
    public interface IExaminationService
    {
        ExaminationDto GetExamination(int studentId);
    }
}