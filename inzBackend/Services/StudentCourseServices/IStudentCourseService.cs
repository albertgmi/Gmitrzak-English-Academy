using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices
{
    public interface IStudentCourseService
    {
        List<StudentAssignmentDto> getStudentsAssignments();
        void completeModule(int matrixModuleId);
        void uncompleteModule(int matrixModuleId);
        List<StudentModuleDto> getSingleModules();
        void completeSingleModule(int id);
        void uncompleteSingleModule(int id);
        List<StudentModuleDto> getCompletedSingleModules();
        StudentModuleDto? getStudentModule(int moduleId);
        void completeStudentModule(int moduleId);
    }
}
