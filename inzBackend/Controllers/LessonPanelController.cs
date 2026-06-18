using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AttendanceModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/lesson-panel")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LessonPanelController : ControllerBase
    {
        private readonly ILessonPanelService _service;

        public LessonPanelController(ILessonPanelService service)
        {
            _service = service;
        }

        [HttpGet("agenda/{studentUserId}")]
        public ActionResult<AgendaDto> GetAgenda([FromRoute] int studentUserId)
        {
            return _service.getAgenda(studentUserId);
        }

        [HttpPut("agenda/{studentUserId}")]
        public ActionResult UpdateAgenda([FromRoute] int studentUserId, [FromBody] UpdateAgendaRequest request)
        {
            _service.updateAgenda(studentUserId, request);
            return Ok();
        }

        [HttpGet("grades/{studentUserId}")]
        public ActionResult<List<LessonGradeDto>> GetGrades([FromRoute] int studentUserId)
        {
            return _service.getGrades(studentUserId);
        }
            
        [HttpGet("activity-points/{studentUserId}")]
        public ActionResult<ActivityPointsLessonSummaryDto> GetActivityPoints([FromRoute] int studentUserId)
        {
            return _service.getActivityPoints(studentUserId);
        }
            
        [HttpPost("activity-points/{studentUserId}")]
        public ActionResult AddActivityPoints([FromRoute] int studentUserId, [FromBody] AddPointsRequest request)
        {
            _service.addActivityPoints(studentUserId, request.Points, request.Reason);
            return Ok();
        }

        [HttpGet("flashcards/{studentUserId}")]
        public ActionResult<LessonFlashcardSummaryDto> GetFlashcards([FromRoute] int studentUserId)
        {
            return _service.getFlashcardSummary(studentUserId);
        }

        [HttpGet("flashcards/all/{studentUserId}")]
        public ActionResult<List<FlashcardDto>> GetAllFlashcardsForUser(int studentUserId)
        {
            return _service.getAllFlashcardsForUser(studentUserId);
        }

        [HttpGet("study-time/{studentUserId}")]
        public ActionResult<StudentStudyTimeDto> GetStudyTime([FromRoute] int studentUserId)
        {
            return _service.getStudyTime(studentUserId);
        }
            
        [HttpGet("last-week/{studentUserId}")]
        public ActionResult<LessonLastWeekDto> GetLastWeek([FromRoute] int studentUserId)
        {
            return _service.getLastWeek(studentUserId);
        }

        [HttpGet("stats/{studentUserId}")]
        public ActionResult<LessonStatsDto> GetStats([FromRoute] int studentUserId)
        {
            return _service.getStats(studentUserId);
        }

        [HttpGet("attendance/{studentUserId}")]
        public ActionResult<IEnumerable<AttendanceDto>> GetAttendance([FromRoute] int studentUserId)
        {
            var records = _service.getAttendance(studentUserId);
            return Ok(records);
        }

        [HttpGet("attendance/{studentUserId}/history")]
        public ActionResult<IEnumerable<AttendanceDto>> GetAttendanceHistory([FromRoute] int studentUserId)
        {
            var records = _service.getAttendanceHistory(studentUserId);
            return Ok(records);
        }

        [HttpPost("attendance")]
        public ActionResult<AttendanceDto> AddAttendance([FromBody] CreateAttendanceDto dto)
        {
            var attendance = _service.addAttendance(dto);
            return Ok(attendance);
        }

        [HttpDelete("attendance/{id}")]
        public ActionResult DeleteAttendance([FromRoute] int id)
        {
            var deleted = _service.deleteAttendance(id);
            return NoContent();
        }

        [HttpPut("flashcards/{studentUserId}/{flashcardId}/interval")]
        public ActionResult UpdateInterval([FromRoute] int studentUserId, [FromRoute] int flashcardId, [FromBody] int interval)
        {
            _service.updateFlashcardInterval(studentUserId, flashcardId, interval);
            return NoContent();
        }
    }
}
