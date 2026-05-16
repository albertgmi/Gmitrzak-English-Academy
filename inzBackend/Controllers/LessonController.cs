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

        [HttpGet("flashcard/search")]
        public SearchGlobalFlashcardResult searchFlashcard(
            [FromQuery] string q, [FromQuery] int studentUserId)
        {
            return _lessonService.searchGlobalFlashcard(q, studentUserId);
        }

        [HttpPost("flashcard/translation")]
        public GlobalFlashcardDto addTranslation([FromBody] AddTranslationRequest request)
        {
            return _lessonService.addTranslation(request);
        }

        [HttpPost("flashcard/assign")]
        public IActionResult assignFlashcard([FromBody] AssignFlashcardToStudentRequest request)
        {
            _lessonService.assignFlashcardToStudent(request);
            return Ok();
        }

        [HttpPost("sentence")]
        public IActionResult addSentence([FromBody] AddSentenceRequest request)
        {
            _lessonService.addSentence(request);
            return Ok();
        }

        [HttpPost("memory")]
        public IActionResult addMemory([FromBody] AddMemoryRequest request)
        {
            _lessonService.addMemory(request);
            return Ok();
        }

        [HttpPost("pronunciation")]
        public IActionResult addPronunciation([FromBody] AddPronunciationRequest request)
        {
            _lessonService.addPronunciation(request);
            return Ok();
        }

        [HttpGet("homework/{studentUserId}")]
        public List<HomeworkItemDto> getHomework(int studentUserId)
        {
            return _lessonService.getHomeworkForWeek(studentUserId);
        }

        [HttpPatch("homework/{id}/check")]
        public IActionResult checkHomework(int id)
        {
            _lessonService.checkHomework(id);
            return Ok();
        }

        [HttpPatch("homework/{id}/uncheck")]
        public IActionResult uncheckHomework(int id)
        {
            _lessonService.uncheckHomework(id);
            return Ok();
        }

        [HttpGet("pronunciation-test/{studentUserId}")]
        public List<PronunciationTestItemDto> getPronunciationTest(int studentUserId)
        {
            return _lessonService.getPronunciationList(studentUserId);
        }

        [HttpPatch("pronunciation-test/{id}/check")]
        public IActionResult checkWord(int id)
        {
            _lessonService.checkPronunciationWord(id);
            return Ok();
        }

        [HttpPatch("pronunciation-test/{id}/uncheck")]
        public IActionResult uncheckWord(int id)
        {
            _lessonService.uncheckPronunciationWord(id);
            return Ok();
        }

        [HttpGet("grades/{studentUserId}")]
        public List<GradeListDto> getGrades(int studentUserId)
        {
            return _lessonService.getGrades(studentUserId);
        }

        [HttpPost("grades")]
        public IActionResult addGrade([FromBody] AddGradeRequest request)
        {
            _lessonService.addGrade(request);
            return Ok();
        }

        [HttpDelete("grades/{gradeId}")]
        public IActionResult removeGrade(int gradeId)
        {
            _lessonService.removeGrade(gradeId);
            return NoContent();
        }

        [HttpGet("notes/{studentUserId}")]
        public List<TeacherNoteDto> getNotes(int studentUserId)
        {
            return _lessonService.getNotes(studentUserId);
        }

        [HttpPost("notes")]
        public IActionResult saveNote([FromBody] SaveNoteRequest request)
        {
            _lessonService.saveNote(request);
            return Ok();
        }

        [HttpDelete("notes/{noteId}")]
        public IActionResult deleteNote(int noteId)
        {
            _lessonService.deleteNote(noteId);
            return NoContent();
        }

        [HttpGet("listening/{studentUserId}")]
        public List<ListeningReportDto> getListeningReports(int studentUserId)
        {
            return _lessonService.getListeningReports(studentUserId);
        }

        [HttpPost("listening")]
        public IActionResult addListeningReport([FromBody] AddListeningReportRequest request)
        {
            _lessonService.addListeningReport(request);
            return Ok();
        }
    }
}
