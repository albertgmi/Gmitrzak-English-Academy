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
        public List<MatrixAssignmentDto> getAllMatrixAssignments()
        {
            return _assignmentService.getAllMatrixAssignments();
        }

        [HttpGet("matrix/user/{userId}")]
        public List<MatrixAssignmentDto> getMatrixAssignmentsByUser([FromRoute] int userId)
        {
            return _assignmentService.getMatrixAssignmentsByUser(userId);
        }

        [HttpPost("matrix")]
        public void createMatrixAssignment([FromBody] CreateMatrixAssignmentRequest request)
        {
            _assignmentService.createMatrixAssignment(request);
        }

        [HttpDelete("matrix/{id}")]
        public void deleteMatrixAssignment([FromRoute] int id)
        {
            _assignmentService.deleteMatrixAssignment(id);
        }

        [HttpGet("module")]
        public List<ModuleAssignmentDto> getAllModuleAssignments()
        {
            return _assignmentService.getAllModuleAssignments();
        }

        [HttpGet("module/user/{userId}")]
        public List<ModuleAssignmentDto> getModuleAssignmentsByUser([FromRoute] int userId)
        {
            return _assignmentService.getModuleAssignmentsByUser(userId);
        }

        [HttpPost("module")]
        public void createModuleAssignment([FromBody] CreateModuleAssignmentRequest request)
        {
            _assignmentService.createModuleAssignment(request);
        }

        [HttpDelete("module/{id}")]
        public void deleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.deleteModuleAssignment(id);
        }

        [HttpPatch("module/{id}/complete")]
        public void completeModuleAssignment([FromRoute] int id)
        {
            _assignmentService.completeModuleAssignment(id);
        }

        [HttpPatch("module/{id}/uncomplete")]
        public void uncompleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.uncompleteModuleAssignment(id);
        }
    }
}
