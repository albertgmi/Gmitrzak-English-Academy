using inzBackend.Models;
using inzBackend.Models.CourseModels;

namespace inzBackend.Services.CourseServices
{
    public interface ICourseService
    {
        List<CourseDto> getAllCourses();
        void updateCourse(int courseId, UpdateCourseRequest request);
        Course createCourse(CreateCourseRequest request);
        void deleteCourse(int courseId);
        void assignProgram(int courseId, int programId);
        void removeProgram(int courseId, int programId);
    }
}
