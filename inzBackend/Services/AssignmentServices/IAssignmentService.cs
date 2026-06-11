using inzBackend.Models.MatrixAssignmentModels;
using inzBackend.Models.ModuleAssignmentModels;

namespace inzBackend.Services.AssignmentServices
{
    public interface IAssignmentService
    {
        List<MatrixAssignmentDto> getAllMatrixAssignments();
        List<MatrixAssignmentDto> getMatrixAssignmentsByUser(int userId);
        void createMatrixAssignment(CreateMatrixAssignmentRequest request);
        void deleteMatrixAssignment(int id);
        List<ModuleAssignmentDto> getAllModuleAssignments();
        List<ModuleAssignmentDto> getModuleAssignmentsByUser(int userId);
        void createModuleAssignment(CreateModuleAssignmentRequest request);
        void deleteModuleAssignment(int id);
        void completeModuleAssignment(int id);
        void uncompleteModuleAssignment(int id);
    }
}
