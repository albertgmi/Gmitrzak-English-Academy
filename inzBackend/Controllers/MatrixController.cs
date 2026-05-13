using inzBackend.Models.MatrixModels;
using inzBackend.Services.MatrixServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/matrix")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class MatrixController : ControllerBase
    {
        private readonly IMatrixService _matrixService;
        public MatrixController(IMatrixService matrixService)
        {
            _matrixService = matrixService;
        }

        [HttpGet]
        public ActionResult<List<MatrixDto>> getAllMatrices()
        {
            return Ok(_matrixService.getAllMatrices());
        }

        [HttpPost]
        public ActionResult createMatrix([FromBody] CreateMatrixRequest request)
        {
            var newMatrix = _matrixService.createMatrix(request);
            return Created();
        }

        [HttpPut("{matrixId}")]
        public ActionResult updateMatrix([FromRoute] int matrixId, [FromBody] UpdateMatrixRequest request)
        {
            _matrixService.updateMatrix(matrixId, request);
            return Ok();
        }

        [HttpDelete("{matrixId}")]
        public ActionResult deleteMatrix([FromRoute] int matrixId)
        {
            _matrixService.deleteMatrix(matrixId);
            return NoContent();
        }

        [HttpPost("{matrixId}/courses/{courseId}")]
        public ActionResult assignCourse(int matrixId, int courseId)
        {
            _matrixService.assignCourse(matrixId, courseId);
            return Ok();
        }

        [HttpDelete("{matrixId}/courses/{courseId}")]
        public ActionResult detachCourse(int matrixId, int courseId)
        {
            _matrixService.detachCourse(matrixId, courseId);
            return Ok();
        }
    }
}
