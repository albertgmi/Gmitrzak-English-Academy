using inzBackend.Models.AdminLearningModels;
using inzBackend.Services.AdminLearningServices.Memories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/admin/memories")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminMemoriesController : ControllerBase
    {
        private readonly IAdminMemoriesService _adminMemoriesService;

        public AdminMemoriesController(IAdminMemoriesService adminMemoriesService)
        {
            _adminMemoriesService = adminMemoriesService;
        }

        [HttpGet("student/{studentId}")]
        public ActionResult<List<AdminMemoryDto>> GetStudentMemories([FromRoute] int studentId)
        {
            return Ok(_adminMemoriesService.GetStudentMemories(studentId));
        }

        [HttpPut("{id}")]
        public ActionResult UpdateMemory([FromRoute] int id, [FromBody] UpdateMemoryRequest request)
        {
            _adminMemoriesService.UpdateMemory(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteMemory([FromRoute] int id)
        {
            _adminMemoriesService.DeleteMemory(id);
            return NoContent();
        }

        [HttpPost("import/{studentId}")]
        public ActionResult<object> ImportMemories([FromRoute] int studentId, [FromForm] IFormFile file)
        {
            var count = _adminMemoriesService.ImportMemoriesFromExcel(studentId, file);
            return Ok(new { count });
        }
    }
}
