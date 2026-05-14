using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices.Grade
{
    public interface IGradesService
    {
        List<GradeDto> getGrades();
    }
}