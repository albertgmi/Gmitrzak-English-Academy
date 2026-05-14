using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices
{
    public interface IGradesService
    {
        List<GradeDto> getGrades();
    }
}