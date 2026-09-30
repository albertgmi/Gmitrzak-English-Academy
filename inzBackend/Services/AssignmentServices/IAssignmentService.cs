using inzBackend.Models.AssignmentModels;
using inzBackend.Models.CourseModels;
using inzBackend.Models.MatrixAssignmentModels;
using inzBackend.Models.ModuleAssignmentModels;
namespace inzBackend.Services.AssignmentServices
{
    public interface IAssignmentService
    {
        List<MatrixAssignmentDto> GetAllMatrixAssignments();
        List<MatrixAssignmentDto> GetMatrixAssignmentsByUser(int userId);
        BulkAssignmentResultDto CreateBulkMatrixAssignment(CreateBulkMatrixAssignmentRequest request);
        BulkAssignmentResultDto CreateCourseAssignment(CreateCourseAssignmentRequest request);
        void DeleteMatrixAssignment(int id);
        int DeleteBulkMatrixAssignments(DeleteBulkMatrixAssignmentsRequest request);
        List<ModuleAssignmentDto> GetAllModuleAssignments();
        List<ModuleAssignmentDto> GetModuleAssignmentsByUser(int userId);
        void CreateModuleAssignment(CreateModuleAssignmentRequest request);
        BulkAssignmentResultDto CreateBulkModuleAssignment(CreateBulkModuleAssignmentRequest request);
        void DeleteModuleAssignment(int id);
        void CompleteModuleAssignment(int id);
        void UncompleteModuleAssignment(int id);
    }
}
