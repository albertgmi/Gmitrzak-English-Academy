using inzBackend.Models.ProgramModels;
using inzBackend.Services.ProgramServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/program")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _programService;
        public ProgramController(IProgramService programService)
        {
            _programService = programService;
        }

        [HttpGet]
        public ActionResult<ProgramDto> getAllPrograms()
        {
            var programs = _programService.getAllPrograms();
            return Ok(programs);
        }

        [HttpPut("{programId}")]
        public ActionResult updateProgram([FromRoute] int programId, [FromBody] UpdateProgramRequest request)
        {
            _programService.updateProgram(programId, request);
            return Ok();
        }

        [HttpDelete("{programId}")]
        public ActionResult deleteProgram([FromRoute] int programId)
        {
            _programService.deleteProgram(programId);
            return NoContent();
        }

        [HttpPost]
        public ActionResult<Entities.Curriculum.Program> createProgram([FromBody] CreateProgramRequest request)
        {
            var createdProgram = _programService.createProgram(request);
            return Ok(createdProgram);
        }

    }
}
