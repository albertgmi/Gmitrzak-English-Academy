using inzBackend.Models.MatrixAssignmentModels;
using inzBackend.Models.ModuleAssignmentModels;
using inzBackend.Services.AssignmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/assignment")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }


        [HttpGet("matrix")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<MatrixAssignmentDto>> GetAllMatrixAssignments()
        {
            return _assignmentService.GetAllMatrixAssignments();
        }

        [HttpGet("matrix/user/{userId}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<List<MatrixAssignmentDto>> GetMatrixAssignmentsByUser([FromRoute] int userId)
        {
            return _assignmentService.GetMatrixAssignmentsByUser(userId);
        }

        [HttpPost("matrix")]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateMatrixAssignment([FromBody] CreateMatrixAssignmentRequest request)
        {
            _assignmentService.CreateMatrixAssignment(request);
            return Created();
        }

        [HttpDelete("matrix/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteMatrixAssignment([FromRoute] int id)
        {
            _assignmentService.DeleteMatrixAssignment(id);
            return NoContent();
        }

        [HttpGet("module")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<ModuleAssignmentDto>> GetAllModuleAssignments()
        {
            return _assignmentService.GetAllModuleAssignments();
        }

        [HttpGet("module/user/{userId}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<List<ModuleAssignmentDto>> GetModuleAssignmentsByUser([FromRoute] int userId)
        {
            return _assignmentService.GetModuleAssignmentsByUser(userId);
        }

        [HttpPost("module")]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateModuleAssignment([FromBody] CreateModuleAssignmentRequest request)
        {
            _assignmentService.CreateModuleAssignment(request);
            return Created();
        }

        [HttpDelete("module/{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.DeleteModuleAssignment(id);
            return NoContent();
        }

        [HttpPatch("module/{id}/complete")]
        [Authorize(Roles = "Admin")]
        public ActionResult CompleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.CompleteModuleAssignment(id);
            return Ok();
        }

        [HttpPatch("module/{id}/uncomplete")]
        [Authorize(Roles = "Admin")]
        public ActionResult UncompleteModuleAssignment([FromRoute] int id)
        {
            _assignmentService.UncompleteModuleAssignment(id);
            return Ok();
        }
    }
}
