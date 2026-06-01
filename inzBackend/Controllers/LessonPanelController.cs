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
        public AgendaDto getAgenda([FromRoute] int studentUserId)
        {
            return _service.getAgenda(studentUserId);
        }

        [HttpPut("agenda/{studentUserId}")]
        public ActionResult updateAgenda([FromRoute] int studentUserId, [FromBody] UpdateAgendaRequest request)
        {
            _service.updateAgenda(studentUserId, request);
            return Ok();
        }

        [HttpGet("grades/{studentUserId}")]
        public List<LessonGradeDto> getGrades([FromRoute] int studentUserId)
        {
            return _service.getGrades(studentUserId);
        }
            
        [HttpGet("activity-points/{studentUserId}")]
        public ActivityPointsLessonSummaryDto getActivityPoints([FromRoute] int studentUserId)
        {
            return _service.getActivityPoints(studentUserId);
        }
            
        [HttpPost("activity-points/{studentUserId}")]
        public ActionResult addActivityPoints([FromRoute] int studentUserId, [FromBody] AddPointsRequest request)
        {
            _service.addActivityPoints(studentUserId, request.Points, request.Reason);
            return Ok();
        }

        [HttpGet("flashcards/{studentUserId}")]
        public LessonFlashcardSummaryDto getFlashcards([FromRoute] int studentUserId)
        {
            return _service.getFlashcardSummary(studentUserId);
        }

        [HttpGet("study-time/{studentUserId}")]
        public StudentStudyTimeDto getStudyTime([FromRoute] int studentUserId)
        {
            return _service.getStudyTime(studentUserId);
        }
            
        [HttpGet("last-week/{studentUserId}")]
        public LessonLastWeekDto getLastWeek([FromRoute] int studentUserId)
        {
            return _service.getLastWeek(studentUserId);
        }

        [HttpGet("stats/{studentUserId}")]
        public LessonStatsDto getStats([FromRoute] int studentUserId)
        {
            return _service.getStats(studentUserId);
        }

        [HttpGet("attendance/{studentUserId}")]
        public ActionResult<IEnumerable<AttendanceDto>> getAttendance([FromRoute] int studentUserId)
        {
            var records = _service.getAttendance(studentUserId);
            return Ok(records);
        }

        [HttpGet("attendance/{studentUserId}/history")]
        public ActionResult<IEnumerable<AttendanceDto>> getAttendanceHistory([FromRoute] int studentUserId)
        {
            var records = _service.getAttendanceHistory(studentUserId);
            return Ok(records);
        }

        [HttpPost("attendance")]
        public ActionResult<AttendanceDto> addAttendance([FromBody] CreateAttendanceDto dto)
        {
            var attendance = _service.addAttendance(dto);
            return Ok(attendance);
        }

        [HttpDelete("attendance/{id}")]
        public ActionResult deleteAttendance([FromRoute] int id)
        {
            var deleted = _service.deleteAttendance(id);
            return NoContent();
        }
    }
}
