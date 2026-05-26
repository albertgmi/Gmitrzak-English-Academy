using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AttendanceModels;
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
        public AgendaDto getAgenda(int studentUserId)
        {
            return _service.getAgenda(studentUserId);
        }

        [HttpPut("agenda/{studentUserId}")]
        public IActionResult updateAgenda(int studentUserId, [FromBody] UpdateAgendaRequest request)
        {
            _service.updateAgenda(studentUserId, request);
            return Ok();
        }

        [HttpGet("grades/{studentUserId}")]
        public List<LessonGradeDto> getGrades(int studentUserId)
        {
            return _service.getGrades(studentUserId);
        }
            
        [HttpGet("activity-points/{studentUserId}")]
        public ActivityPointsLessonSummaryDto getActivityPoints(int studentUserId)
        {
            return _service.getActivityPoints(studentUserId);
        }
            
        [HttpPost("activity-points/{studentUserId}")]
        public IActionResult addActivityPoints(int studentUserId, [FromBody] AddPointsRequest request)
        {
            _service.addActivityPoints(studentUserId, request.Points, request.Reason);
            return Ok();
        }

        [HttpGet("flashcards/{studentUserId}")]
        public LessonFlashcardSummaryDto getFlashcards(int studentUserId)
        {
            return _service.getFlashcardSummary(studentUserId);
        }

        [HttpGet("study-time/{studentUserId}")]
        public StudentStudyTimeDto getStudyTime(int studentUserId)
        {
            return _service.getStudyTime(studentUserId);
        }
            
        [HttpGet("last-week/{studentUserId}")]
        public LessonLastWeekDto getLastWeek(int studentUserId)
        {
            return _service.getLastWeek(studentUserId);
        }

        [HttpGet("stats/{studentUserId}")]
        public LessonStatsDto getStats(int studentUserId)
        {
            return _service.getStats(studentUserId);
        }

        [HttpGet("attendance/{studentUserId}")]
        public ActionResult<IEnumerable<AttendanceDto>> GetAttendance([FromRoute] int studentUserId)
        {
            var records = _service.getAttendance(studentUserId);
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
    }
}
