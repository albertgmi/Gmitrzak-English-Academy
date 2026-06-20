using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices
{
    public interface IStudentCourseService
    {
        List<StudentAssignmentDto> GetStudentsAssignments();
        void CompleteModule(int matrixModuleId);
        void UncompleteModule(int matrixModuleId);
        List<StudentModuleDto> GetSingleModules();
        void CompleteSingleModule(int id);
        void UncompleteSingleModule(int id);
        List<StudentModuleDto> GetCompletedSingleModules();
        StudentModuleDto? GetStudentModule(int moduleId);
        void CompleteStudentModule(int moduleId);
    }
}
