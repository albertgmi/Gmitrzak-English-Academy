using inzBackend.Models.CourseModels;

namespace inzBackend.Services.CourseServices
{
    public interface ICourseService
    {
        public List<CourseDto> getAllCourses();
        public void updateCourse(int courseId, UpdateCourseRequest request);
        public void deleteCourse(int courseId);
        public void assignProgram(int courseId, int programId);
        public void removeProgram(int courseId, int programId);
    }
}
