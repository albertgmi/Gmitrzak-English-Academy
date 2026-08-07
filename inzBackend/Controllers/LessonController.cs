using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AiSpellCheckingModels;
using inzBackend.Models.StudentLearningModels.AlphabetModels;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Services.AdminLearningServices.Lesson;
using inzBackend.Services.AiIntegrationServices;
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
        private readonly IAiSpellCheckService _aiSpellCheckService;

        public LessonController(ILessonService lessonService, IAiSpellCheckService aiSpellCheckService)
        {
            _lessonService = lessonService;
            _aiSpellCheckService = aiSpellCheckService;
        }

        [HttpPost("sentence")]
        public ActionResult AddSentence([FromBody] AddSentenceRequest request)
        {
            _lessonService.AddSentence(request);
            return Ok();
        }

        [HttpPost("memory")]
        public ActionResult AddMemory([FromBody] AddMemoryRequest request)
        {
            _lessonService.AddMemory(request);
            return Ok();
        }

        [HttpPost("pronunciation")]
        public ActionResult AddPronunciation([FromBody] AddPronunciationRequest request)
        {
            _lessonService.AddPronunciation(request);
            return Ok();
        }

        [HttpGet("homework/{studentUserId}")]
        public ActionResult<List<HomeworkItemDto>> GetHomework([FromRoute] int studentUserId)
        {
            var result = _lessonService.GetHomeworkForWeek(studentUserId);
            return Ok(result);
        }

        [HttpPatch("homework/{id}/check")]
        public ActionResult CheckHomework([FromRoute] int id)
        {
            _lessonService.CheckHomework(id);
            return Ok();
        }

        [HttpPatch("homework/{id}/uncheck")]
        public ActionResult UncheckHomework([FromRoute] int id)
        {
            _lessonService.UncheckHomework(id);
            return Ok();
        }

        [HttpGet("pronunciation-test/{studentUserId}")]
        public ActionResult<List<PronunciationTestItemDto>> GetPronunciationTest([FromRoute] int studentUserId)
        {
            var result = _lessonService.GetPronunciationList(studentUserId);
            return Ok(result);
        }

        [HttpPatch("pronunciation-test/{id}/check")]
        public ActionResult CheckWord([FromRoute] int id)
        {
            _lessonService.CheckPronunciationWord(id);
            return Ok();
        }

        [HttpPatch("pronunciation-test/{id}/uncheck")]
        public ActionResult UncheckWord([FromRoute] int id)
        {
            _lessonService.UncheckPronunciationWord(id);
            return Ok();
        }

        [HttpGet("pronunciation/correct/{studentId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<PronunciationTestItemDto>> GetCorrectEntries(int studentId)
        {
            return Ok(_lessonService.GetCorrectEntries(studentId));
        }

        [HttpGet("grades/{studentUserId}")]
        public ActionResult<List<GradeListDto>> GetGrades([FromRoute] int studentUserId)
        {
            var result = _lessonService.GetGrades(studentUserId);
            return Ok(result);
        }

        [HttpPost("grades")]
        public ActionResult AddGrade([FromBody] AddGradeRequest request)
        {
            _lessonService.AddGrade(request);
            return Ok();
        }

        [HttpDelete("grades/{gradeId}")]
        public ActionResult RemoveGrade([FromRoute] int gradeId)
        {
            _lessonService.RemoveGrade(gradeId);
            return NoContent();
        }

        [HttpGet("notes/{studentUserId}")]
        public ActionResult<List<TeacherNoteDto>> GetNotes([FromRoute] int studentUserId)
        {
            var result = _lessonService.GetNotes(studentUserId);
            return Ok(result);
        }

        [HttpPost("notes")]
        public ActionResult SaveNote([FromBody] SaveNoteRequest request)
        {
            _lessonService.SaveNote(request);
            return Ok();
        }

        [HttpDelete("notes/{noteId}")]
        public ActionResult DeleteNote([FromRoute] int noteId)
        {
            _lessonService.DeleteNote(noteId);
            return NoContent();
        }

        [HttpGet("listening/{studentUserId}")]
        public ActionResult<List<ListeningReportDto>> GetListeningReports([FromRoute] int studentUserId)
        {
            var result = _lessonService.GetListeningReports(studentUserId);
            return Ok(result);
        }

        [HttpPost("listening")]
        public ActionResult AddListeningReport([FromBody] AddListeningReportRequest request)
        {
            _lessonService.AddListeningReport(request);
            return Ok();
        }

        [HttpGet("memory/{studentUserId}")]
        public ActionResult<List<MemoryDto>> GetMemories([FromRoute] int studentUserId)
        {
            var result = _lessonService.GetMemories(studentUserId);
            return Ok(result);
        }

        [HttpPost("pronunciation/mark")]
        [Authorize(Roles = "Admin")]
        public ActionResult MarkPronunciation([FromBody] MarkPronunciationRequest request)
        {
            _lessonService.MarkPronunciationResult(request);
            return Ok();
        }

        [HttpPost("spellcheck")]
        public async Task<ActionResult<SpellCheckResult>> SpellCheck([FromBody] SpellCheckRequest request)
        {
            var result = await _aiSpellCheckService.CheckTextAsync(request.Text, request.Language ?? "English");
            return Ok(result);
        }

        [HttpPost("alphabet/pool")]
        public ActionResult AddAlphabetAbbreviation([FromBody] AddAlphabetAbbreviationRequest request)
        {
            _lessonService.AddAlphabetAbbreviation(request);
            return Ok();
        }

        [HttpGet("alphabet/pool")]
        public ActionResult<List<AlphabetAbbreviationDto>> GetAlphabetPool()
        {
            return Ok(_lessonService.GetAlphabetPool());
        }

        [HttpDelete("alphabet/pool/{id}")]
        public ActionResult DeleteAlphabetAbbreviation([FromRoute] int id)
        {
            _lessonService.DeleteAlphabetAbbreviation(id);
            return NoContent();
        }

        [HttpGet("alphabet-test/{studentUserId}")]
        public ActionResult<List<AlphabetTestItemDto>> GetAlphabetTest([FromRoute] int studentUserId)
        {
            return Ok(_lessonService.GetAlphabetTestList(studentUserId));
        }

        [HttpGet("alphabet-test/history/{studentUserId}")]
        public ActionResult<List<AlphabetHistoryItemDto>> GetAlphabetHistory([FromRoute] int studentUserId)
        {
            return Ok(_lessonService.GetAlphabetHistory(studentUserId));
        }

        [HttpPost("alphabet/mark")]
        public ActionResult MarkAlphabet([FromBody] MarkAlphabetRequest request)
        {
            _lessonService.MarkAlphabetResult(request);
            return Ok();
        }

        [HttpGet("alphabet-test/{entryId}/attempts")]
        public ActionResult<List<AlphabetAttemptDto>> GetAlphabetEntryAttempts([FromRoute] int entryId)
        {
            return Ok(_lessonService.GetAlphabetEntryAttempts(entryId));
        }
    }
}
