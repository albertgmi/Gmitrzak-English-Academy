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
        public ActionResult<List<MatrixAssignmentDto>> GetAllMatrixAssignments()
        {
            return _assignmentService.getAllMatrixAssignments();
        }

        [HttpGet("matrix/user/{userId}")]
        public ActionResult<List<MatrixAssignmentDto>> GetMatrixAssignmentsByUser([FromRoute] int userId)
        {
            return _assignmentService.getMatrixAssignmentsByUser(userId);
        }

        [HttpPost("matrix")]
        public ActionResult CreateMatrixAssignment([FromBody] CreateMatrixAssignmentRequest request)
        {
            _assignmentService.createMatrixAssignment(request);
            return Created();
        }

        [HttpDelete("matrix/{id}")]
        public ActionResult DeleteMatrixAssignment([FromRoute] int id)
        {
            _assignmentService.deleteMatrixAssignment(id);
            return NoContent();
        }

        [HttpGet("module")]
        public ActionResult<List<ModuleAssignmentDto>> GetAllModuleAssignments()
        {
            return _assignmentService.getAllModuleAssignments();
        }

        [HttpGet("module/user/{userId}")]
        public ActionResult<List<ModuleAssignmentDto>> GetModuleAssignmentsByUser([FromRoute] int userId)
        {
            return _assignmentService.getModuleAssignmentsByUser(userId);
        }

        [HttpPost("module")]
        public ActionResult CreateModuleAssignment([FromBody] CreateModuleAssignmentRequest request)
        {
            _assignmentService.createModuleAssignment(request);
            return Created();
        }

        [HttpDelete("module/{id}")]
        public ActionResult DeleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.deleteModuleAssignment(id);
            return NoContent();
        }

        [HttpPatch("module/{id}/complete")]
        public ActionResult CompleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.completeModuleAssignment(id);
            return Ok();
        }

        [HttpPatch("module/{id}/uncomplete")]
        public ActionResult UncompleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.uncompleteModuleAssignment(id);
            return Ok();
        }
    }
}
