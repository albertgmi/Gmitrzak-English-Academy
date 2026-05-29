using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.ModuleServices
{
    public interface IModuleService
    {
        List<ModuleDto> getAllModules();
        List<ModuleDto> getSentenceModulesForStudent(int studentId);
        ModuleDto createModule(CreateModuleRequest request);
        void deleteModule(int moduleId);
        void updateModule(int moduleId, UpdateModuleRequest request);
        void assignMatrix(int moduleId, int matrixId, AssignModuleToMatrixRequest request);
        void detachMatrix(int moduleId, int matrixId);
        StudentModuleDto? getStudentModule(int userId, int moduleId);
        void completeStudentModule(int userId, int moduleId);
    }
}
