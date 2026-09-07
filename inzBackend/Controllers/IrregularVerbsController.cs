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

        [HttpGet("all")]
        public ActionResult<List<IrregularVerbDto>> GetAllIrregularVerbs()
        {
            var result = _irregularVerbsService.GetAllIrregularVerbs();
            return Ok(result);
        }

        [HttpGet("leeches")]
        public ActionResult<List<IrregularVerbDto>> GetLeeches()
        {
            var result = _irregularVerbsService.GetLeeches();
            return Ok(result);
        }

        [HttpGet("studied-today")]
        public ActionResult<List<IrregularVerbDto>> GetStudiedToday()
        {
            var result = _irregularVerbsService.GetStudiedToday();
            return Ok(result);
        }

        [HttpGet("search")]
        public ActionResult<List<IrregularVerbDto>> SearchIrregularVerbs([FromQuery] string q)
        {
            var result = _irregularVerbsService.SearchIrregularVerbs(q);
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
