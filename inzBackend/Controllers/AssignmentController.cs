using inzBackend.Models.MatrixAssignmentModels;
using inzBackend.Models.ModuleAssignmentModels;
using inzBackend.Services.AssignmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/assignment")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }


        [HttpGet("matrix")]
        public List<MatrixAssignmentDto> GetAllMatrixAssignments()
        {
            return _assignmentService.GetAllMatrixAssignments();
        }

        [HttpGet("matrix/user/{userId}")]
        public List<MatrixAssignmentDto> GetMatrixAssignmentsByUser(int userId)
        {
            return _assignmentService.GetMatrixAssignmentsByUser(userId);
        }

        [HttpPost("matrix")]
        public void CreateMatrixAssignment([FromBody] CreateMatrixAssignmentRequest request)
        {
            _assignmentService.CreateMatrixAssignment(request);
        }

        [HttpDelete("matrix/{id}")]
        public void DeleteMatrixAssignment(int id)
        {
            _assignmentService.DeleteMatrixAssignment(id);
        }

        [HttpGet("module")]
        public List<ModuleAssignmentDto> GetAllModuleAssignments()
        {
            return _assignmentService.GetAllModuleAssignments();
        }

        [HttpGet("module/user/{userId}")]
        public List<ModuleAssignmentDto> GetModuleAssignmentsByUser(int userId)
        {
            return _assignmentService.GetModuleAssignmentsByUser(userId);
        }

        [HttpPost("module")]
        public void CreateModuleAssignment([FromBody] CreateModuleAssignmentRequest request)
        {
            _assignmentService.CreateModuleAssignment(request);
        }

        [HttpDelete("module/{id}")]
        public void DeleteModuleAssignment(int id)
        {
            _assignmentService.DeleteModuleAssignment(id);
        }

        [HttpPatch("module/{id}/complete")]
        public void CompleteModuleAssignment(int id)
        {
            _assignmentService.CompleteModuleAssignment(id);
        }

        [HttpPatch("module/{id}/uncomplete")]
        public void UncompleteModuleAssignment(int id)
        {
            _assignmentService.UncompleteModuleAssignment(id);
        }
    }
}
