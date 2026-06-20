using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.ModuleServices
{
    public interface IModuleService
    {
        List<ModuleDto> GetAllModules();
        List<ModuleDto> GetSentenceModulesForStudent(int studentId);
        ModuleDto CreateModule(CreateModuleRequest request);
        void DeleteModule(int moduleId);
        void UpdateModule(int moduleId, UpdateModuleRequest request);
        void AssignMatrix(int moduleId, int matrixId, AssignModuleToMatrixRequest request);
        void DetachMatrix(int moduleId, int matrixId);
    }
}
