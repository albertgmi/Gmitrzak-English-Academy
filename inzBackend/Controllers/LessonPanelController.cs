using inzBackend.Models.AdminLearningModels;
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

        [HttpGet("stream/{studentUserId}")]
        public List<StreamEntryDto> getStream(int studentUserId)
        {
            return _service.getStreamEntries(studentUserId);
        }
            
        [HttpPost("stream/{studentUserId}")]
        public IActionResult addStream(int studentUserId, [FromBody] AddStreamRequest request)
        {
            _service.addStreamEntry(studentUserId, request.Command, request.Payload);
            return Ok();
        }

        [HttpDelete("stream/{entryId}")]
        public IActionResult deleteStream(int entryId)
        {
            _service.deleteStreamEntry(entryId);
            return NoContent();
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
    }
}
