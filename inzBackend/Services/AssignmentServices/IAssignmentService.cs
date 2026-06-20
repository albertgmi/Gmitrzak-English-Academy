using inzBackend.Models.MatrixAssignmentModels;
using inzBackend.Models.ModuleAssignmentModels;

namespace inzBackend.Services.AssignmentServices
{
    public interface IAssignmentService
    {
        List<MatrixAssignmentDto> GetAllMatrixAssignments();
        List<MatrixAssignmentDto> GetMatrixAssignmentsByUser(int userId);
        void CreateMatrixAssignment(CreateMatrixAssignmentRequest request);
        void DeleteMatrixAssignment(int id);
        List<ModuleAssignmentDto> GetAllModuleAssignments();
        List<ModuleAssignmentDto> GetModuleAssignmentsByUser(int userId);
        void CreateModuleAssignment(CreateModuleAssignmentRequest request);
        void DeleteModuleAssignment(int id);
        void CompleteModuleAssignment(int id);
        void UncompleteModuleAssignment(int id);
    }
}
