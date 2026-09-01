using System.Collections.Generic;
using inzBackend.Models.LiveNotepadModels;
using inzBackend.Services.LiveNotepadServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [ApiController]
    [Route("api/live-notepad")]
    [Authorize]
    public class LiveNotepadController : ControllerBase
    {
        private readonly ILiveNotepadService _service;

        public LiveNotepadController(ILiveNotepadService service)
        {
            _service = service;
        }

        [HttpGet("notes")]
        public ActionResult<List<LiveNoteSummaryDto>> GetNotes([FromQuery] int? studentId = null)
        {
            return Ok(_service.GetLiveNotes(studentId));
        }

        [HttpGet("notes/{noteId}")]
        public ActionResult<LiveNoteDetailDto> GetNoteById([FromRoute] int noteId)
        {
            return Ok(_service.GetLiveNoteById(noteId));
        }

        [HttpPost("notes")]
        public ActionResult<LiveNoteDetailDto> CreateNote([FromBody] CreateLiveNoteRequest request)
        {
            return Ok(_service.CreateLiveNote(request));
        }

        [HttpPut("notes/{noteId}")]
        public ActionResult<LiveNoteDetailDto> SaveNote([FromRoute] int noteId, [FromBody] SaveLiveNoteRequest request)
        {
            return Ok(_service.SaveLiveNote(noteId, request));
        }

        [HttpDelete("notes/{noteId}")]
        public ActionResult DeleteNote([FromRoute] int noteId)
        {
            _service.DeleteLiveNote(noteId);
            return NoContent();
        }
    }
}
