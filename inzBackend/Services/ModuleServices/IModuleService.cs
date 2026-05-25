using inzBackend.Models;
using inzBackend.Models.ModuleModels;

namespace inzBackend.Services.ModuleServices
{
    public interface IModuleService
    {
        List<ModuleDto> getAllModules(int studentId);
        Module createModule(CreateModuleRequest request);
        void deleteModule(int moduleId);
        void updateModule(int moduleId, UpdateModuleRequest request);
        void assignMatrix(int moduleId, int matrixId, AssignModuleToMatrixRequest request);
        void detachMatrix(int moduleId, int matrixId);
    }
}
