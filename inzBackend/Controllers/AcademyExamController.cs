using inzBackend.Enums;
using inzBackend.Models.AcademyExamModels;
using inzBackend.Services.AcademyExamServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/academy-exams")]
    [ApiController]
    [Authorize]
    public class AcademyExamController : ControllerBase
    {
        private readonly IAcademyExamService _examService;
        private readonly IUserContextService _userContextService;

        public AcademyExamController(IAcademyExamService examService, IUserContextService userContextService)
        {
            _examService = examService;
            _userContextService = userContextService;
        }

        [HttpGet]
        public ActionResult<List<AcademyExamDto>> GetExams([FromQuery] ExamLevel level = ExamLevel.Junior)
        {
            var userId = _userContextService.GetUserId!.Value;
            var exams = _examService.GetExamsByLevel(level, userId);
            return Ok(exams);
        }

        [HttpGet("{id}")]
        public ActionResult<AcademyExamDto> GetExamById(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            var exam = _examService.GetExamById(id, userId);
            return Ok(exam);
        }

        [HttpPost("{id}/signup")]
        public ActionResult SignUp(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            _examService.SignUpForExam(id, userId);
            return Ok(new { message = "Successfully registered for the exam." });
        }

        [HttpDelete("{id}/signup")]
        public ActionResult Unsign(int id)
        {
            var userId = _userContextService.GetUserId!.Value;
            _examService.UnsignFromExam(id, userId);
            return Ok(new { message = "Successfully cancelled registration for the exam." });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<int> CreateExam([FromBody] CreateAcademyExamDto dto)
        {
            var examId = _examService.CreateExam(dto);
            return Created($"/api/academy-exams/{examId}", new { id = examId });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateExam(int id, [FromBody] UpdateAcademyExamDto dto)
        {
            _examService.UpdateExam(id, dto);
            return Ok(new { message = "Exam updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteExam(int id)
        {
            _examService.DeleteExam(id);
            return Ok(new { message = "Exam deleted successfully." });
        }

        [HttpPost("{id}/takers/{userId}/status")]
        [Authorize(Roles = "Admin")]
        public ActionResult MarkTakerStatus(int id, int userId, [FromBody] MarkExamTakerStatusDto dto)
        {
            _examService.MarkTakerStatus(id, userId, dto.Status);
            return Ok(new { message = "Taker status updated successfully." });
        }
    }
}
