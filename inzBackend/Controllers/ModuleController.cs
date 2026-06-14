using inzBackend.Entities.Curriculum;
using inzBackend.Models.ModuleModels;
using inzBackend.Services.ModuleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/module")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet]
        public ActionResult<List<ModuleDto>> getAllModules()
        {
            return _moduleService.getAllModules();
        }

        [HttpGet("student/{studentId}/sentences")]
        public ActionResult<List<ModuleDto>> getSentenceModulesForStudent([FromRoute] int studentId)
        {
            return Ok(_moduleService.getSentenceModulesForStudent(studentId));
        }

        [HttpPost]
        public ActionResult<Module> createModule([FromBody] CreateModuleRequest request)
        {
            return Ok(_moduleService.createModule(request));
        }

        [HttpPut("{moduleId}")]
        public ActionResult updateModule([FromRoute] int moduleId, [FromBody] UpdateModuleRequest request)
        {
            _moduleService.updateModule(moduleId, request);
            return Ok();
        }

        [HttpDelete("{moduleId}")]
        public ActionResult deleteModule([FromRoute] int moduleId)
        {
            _moduleService.deleteModule(moduleId);
            return NoContent();
        }

        [HttpPost("{moduleId}/matrix/{matrixId}")]
        public ActionResult assignMatrix([FromRoute] int moduleId, [FromRoute] int matrixId, [FromBody] AssignModuleToMatrixRequest request)
        {
            _moduleService.assignMatrix(moduleId, matrixId, request);
            return Ok();
        }

        [HttpDelete("{moduleId}/matrix/{matrixId}")]
        public ActionResult detachMatrix([FromRoute] int moduleId, [FromRoute] int matrixId)
        {
            _moduleService.detachMatrix(moduleId, matrixId);
            return NoContent();
        }
    }
}
