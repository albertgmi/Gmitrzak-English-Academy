using inzBackend.Models.AdminLearningModels;
using inzBackend.Services.AdminLearningServices.Pronunciation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/admin/pronunciation")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPronunciationController : ControllerBase
    {
        private readonly IAdminPronunciationService _adminPronunciationService;

        public AdminPronunciationController(IAdminPronunciationService adminPronunciationService)
        {
            _adminPronunciationService = adminPronunciationService;
        }

        [HttpGet("student/{studentId}")]
        public ActionResult<List<AdminPronunciationDto>> GetStudentPronunciation([FromRoute] int studentId)
        {
            return Ok(_adminPronunciationService.GetStudentPronunciation(studentId));
        }

        [HttpPut("{id}")]
        public ActionResult UpdatePronunciation([FromRoute] int id, [FromBody] UpdatePronunciationRequest request)
        {
            _adminPronunciationService.UpdatePronunciation(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePronunciation([FromRoute] int id)
        {
            _adminPronunciationService.DeletePronunciation(id);
            return NoContent();
        }
    }
}
