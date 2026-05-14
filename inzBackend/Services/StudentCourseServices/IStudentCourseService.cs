using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices
{
    public interface IStudentCourseService
    {
        List<StudentAssignmentDto> getStudentsAssignments();
        void completeModule(int matrixModuleId);
        void uncompleteModule(int matrixModuleId);
    }
}
