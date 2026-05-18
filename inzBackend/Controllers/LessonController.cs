using inzBackend.Entities;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Services.AdminLearningServices.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/lesson")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpPost("sentence")]
        public ActionResult addSentence([FromBody] AddSentenceRequest request)
        {
            _lessonService.addSentence(request);
            return Ok();
        }

        [HttpPost("memory")]
        public ActionResult addMemory([FromBody] AddMemoryRequest request)
        {
            _lessonService.addMemory(request);
            return Ok();
        }

        [HttpPost("pronunciation")]
        public ActionResult addPronunciation([FromBody] AddPronunciationRequest request)
        {
            _lessonService.addPronunciation(request);
            return Ok();
        }

        [HttpGet("homework/{studentUserId}")]
        public ActionResult<List<HomeworkItemDto>> getHomework(int studentUserId)
        {
            var result = _lessonService.getHomeworkForWeek(studentUserId);
            return Ok(result);
        }

        [HttpPatch("homework/{id}/check")]
        public ActionResult checkHomework(int id)
        {
            _lessonService.checkHomework(id);
            return Ok();
        }

        [HttpPatch("homework/{id}/uncheck")]
        public ActionResult uncheckHomework(int id)
        {
            _lessonService.uncheckHomework(id);
            return Ok();
        }

        [HttpGet("pronunciation-test/{studentUserId}")]
        public ActionResult<List<PronunciationTestItemDto>> getPronunciationTest(int studentUserId)
        {
            var result = _lessonService.getPronunciationList(studentUserId);
            return Ok(result);
        }

        [HttpPatch("pronunciation-test/{id}/check")]
        public ActionResult checkWord(int id)
        {
            _lessonService.checkPronunciationWord(id);
            return Ok();
        }

        [HttpPatch("pronunciation-test/{id}/uncheck")]
        public ActionResult uncheckWord(int id)
        {
            _lessonService.uncheckPronunciationWord(id);
            return Ok();
        }

        [HttpGet("grades/{studentUserId}")]
        public ActionResult<List<GradeListDto>> getGrades(int studentUserId)
        {
            var result = _lessonService.getGrades(studentUserId);
            return Ok(result);
        }

        [HttpPost("grades")]
        public ActionResult addGrade([FromBody] AddGradeRequest request)
        {
            _lessonService.addGrade(request);
            return Ok();
        }

        [HttpDelete("grades/{gradeId}")]
        public ActionResult removeGrade(int gradeId)
        {
            _lessonService.removeGrade(gradeId);
            return NoContent();
        }

        [HttpGet("notes/{studentUserId}")]
        public ActionResult<List<TeacherNoteDto>> getNotes(int studentUserId)
        {
            var result = _lessonService.getNotes(studentUserId);
            return Ok(result);
        }

        [HttpPost("notes")]
        public ActionResult saveNote([FromBody] SaveNoteRequest request)
        {
            _lessonService.saveNote(request);
            return Ok();
        }

        [HttpDelete("notes/{noteId}")]
        public ActionResult deleteNote(int noteId)
        {
            _lessonService.deleteNote(noteId);
            return NoContent();
        }

        [HttpGet("listening/{studentUserId}")]
        public ActionResult<List<ListeningReportDto>> getListeningReports(int studentUserId)
        {
            var result = _lessonService.getListeningReports(studentUserId);
            return Ok(result);
        }

        [HttpPost("listening")]
        public ActionResult addListeningReport([FromBody] AddListeningReportRequest request)
        {
            _lessonService.addListeningReport(request);
            return Ok();
        }
    }
}
