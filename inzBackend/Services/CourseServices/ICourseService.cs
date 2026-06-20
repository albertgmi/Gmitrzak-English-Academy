using inzBackend.Entities.Curriculum;
using inzBackend.Models.CourseModels;

namespace inzBackend.Services.CourseServices
{
    public interface ICourseService
    {
        List<CourseDto> GetAllCourses();
        void UpdateCourse(int courseId, UpdateCourseRequest request);
        Course CreateCourse(CreateCourseRequest request);
        void DeleteCourse(int courseId);
        void AssignProgram(int courseId, int programId);
        void RemoveProgram(int courseId, int programId);
    }
}
