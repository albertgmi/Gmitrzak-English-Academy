using AutoMapper;
using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Models.StudentCourseModels;
using inzBackend.Services.ModuleServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/module")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly IUserContextService _userContextService;
        public ModuleController(IModuleService moduleService, IMapper mapper, IUserContextService userContextService)
        {
            _moduleService = moduleService;
            _userContextService = userContextService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<ModuleDto>> getAllModules()
        {
            return _moduleService.getAllModules();
        }

        [HttpGet("student/{studentId}/sentences")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<ModuleDto>> getSentenceModulesForStudent([FromRoute] int studentId)
        {
            return Ok(_moduleService.getSentenceModulesForStudent(studentId));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<Module> createModule([FromBody] CreateModuleRequest request)
        {
            return Ok(_moduleService.createModule(request));
        }

        [HttpPut("{moduleId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult updateModule([FromRoute] int moduleId, [FromBody] UpdateModuleRequest request)
        {
            _moduleService.updateModule(moduleId, request);
            return Ok();
        }

        [HttpDelete("{moduleId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult deleteModule([FromRoute] int moduleId)
        {
            _moduleService.deleteModule(moduleId);
            return NoContent();
        }

        [HttpPost("{moduleId}/matrix/{matrixId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult assignMatrix([FromRoute] int moduleId, [FromRoute] int matrixId, [FromBody] AssignModuleToMatrixRequest request)
        {
            _moduleService.assignMatrix(moduleId, matrixId, request);
            return Ok();
        }

        [HttpDelete("{moduleId}/matrix/{matrixId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult detachMatrix([FromRoute] int moduleId, [FromRoute] int matrixId)
        {
            _moduleService.detachMatrix(moduleId, matrixId);
            return NoContent();
        }
        // todo przeniesc do kontrolera studenta nie admina 
        [HttpGet("student-module/{moduleId}")]
        [Authorize(Roles = "User")]
        public ActionResult<StudentModuleDto> getStudentModule(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;
            var result = _moduleService.getStudentModule(userId, moduleId);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("student-module/{assignmentId}/toggle-complete")]
        [Authorize(Roles = "User")]
        public IActionResult toggleComplete(int assignmentId)
        {
            var userId = _userContextService.GetUserId!.Value;
            _moduleService.toggleComplete(userId, assignmentId);
            return Ok();
        }

    }
}
