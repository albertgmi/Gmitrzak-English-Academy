using inzBackend.Enums;
using inzBackend.Models.StudentLearningModels.IrregularVerbModels;
using inzBackend.Services.StudentLearningServices.IrregularVerbs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/student-learning/irregular-verbs")]
    [Authorize(Roles = "User,Admin")]
    [ApiController]
    public class IrregularVerbsController : ControllerBase
    {
        private readonly IIrregularVerbsService _irregularVerbsService;

        public IrregularVerbsController(IIrregularVerbsService irregularVerbsService)
        {
            _irregularVerbsService = irregularVerbsService;
        }

        [HttpGet]
        public ActionResult<List<IrregularVerbDto>> GetIrregularVerbs([FromQuery] IrregularVerbLevel level = IrregularVerbLevel.Basic)
        {
            var result = _irregularVerbsService.GetIrregularVerbsByLevel(level);
            return Ok(result);
        }

        [HttpPatch("{id}/review")]
        public ActionResult ReviewIrregularVerb([FromRoute] int id, [FromBody] ReviewIrregularVerbRequest request)
        {
            _irregularVerbsService.ReviewIrregularVerb(id, request);
            return Ok();
        }
    }
}
