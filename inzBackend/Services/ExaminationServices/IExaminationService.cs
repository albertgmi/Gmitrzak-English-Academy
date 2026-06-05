using inzBackend.Models.ExaminationModels;

namespace inzBackend.Services.ExaminationServices
{
    public interface IExaminationService
    {
        ExaminationDto getExamination(int studentId);
    }
}