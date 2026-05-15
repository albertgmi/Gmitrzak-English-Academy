using inzBackend.Models.StudentLearningModels.AssignmentStudentModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;
using inzBackend.Models.StudentLearningModels.SentenceModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Services.StudentLearningServices.Assignment;
using inzBackend.Services.StudentLearningServices.Flashcards;
using inzBackend.Services.StudentLearningServices.Memories;
using inzBackend.Services.StudentLearningServices.Pronunciation;
using inzBackend.Services.StudentLearningServices.Sentences;
using inzBackend.Services.StudentLearningServices.Vocabulary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/student-learning")]
    [Authorize(Roles = "User")]
    [ApiController]
    public class StudentLearningController : ControllerBase
    {
        private readonly ISentencesService _sentencesService;
        private readonly IMemoriesService _memoriesService;
        private readonly IPronunciationService _pronunciationService;
        private readonly IFlashcardsService _flashcardsService;
        private readonly IVocabularyService _vocabularyService;
        private readonly IStudentAssignmentService _studentAssignmentService;

        public StudentLearningController(ISentencesService sentencesService, IMemoriesService memoriesService,
            IPronunciationService pronunciationService, IFlashcardsService flashcardsService, IVocabularyService vocabularyService,
            IStudentAssignmentService studentAssignmentService)
        {
            _sentencesService = sentencesService;
            _memoriesService = memoriesService;
            _pronunciationService = pronunciationService;
            _flashcardsService = flashcardsService;
            _vocabularyService = vocabularyService;
            _studentAssignmentService = studentAssignmentService;
        }

        [HttpGet("sentences")]
        public ActionResult<List<SentenceDto>> getAllSentences()
        {
            return _sentencesService.getAllSentences();
        }

        [HttpGet("memories")]
        public ActionResult<List<MemoryDto>> getAllMemories()
        {
            return _memoriesService.getAllMemories();
        }

        [HttpGet("pronunciation")]
        public ActionResult<List<PronunciationEntryDto>> getAllPronunciation()
        {
            return _pronunciationService.getAllEntries();
        }

        [HttpPatch("pronunciation/{id}/check")]
        public IActionResult checkPronunciation(int id)
        {
            _pronunciationService.checkEntry(id);
            return Ok();
        }

        [HttpPatch("pronunciation/{id}/uncheck")]
        public IActionResult uncheckPronunciation(int id)
        {
            _pronunciationService.uncheckEntry(id);
            return Ok();
        }

        [HttpGet("flashcards")]
        public ActionResult<List<FlashcardDto>> getAllFlashcards()
        {
            return _flashcardsService.getAllFlashcards();
        }

        [HttpGet("flashcards/leeches")]
        public ActionResult<List<FlashcardDto>> getLeeches()
        {
            return _flashcardsService.getLeeches();
        }

        [HttpGet("flashcards/studied-today")]
        public ActionResult<List<FlashcardDto>> getStudiedToday()
        {
            return _flashcardsService.getStudiedToday();
        }

        [HttpGet("flashcards/logs")]
        public ActionResult<List<FlashcardStudyLogDto>> getStudyLogs()
        {
            return _flashcardsService.getStudyLogs();
        }

        [HttpGet("flashcards/search")]
        public ActionResult<List<FlashcardDto>> searchFlashcards([FromQuery] string q)
        {
            return _flashcardsService.searchFlashcards(q);
        }

        [HttpGet("vocabulary")]
        public ActionResult<List<VocabularyDto>> getAllVocabulary()
        {
            return _vocabularyService.getAllVocabulary();
        }

        [HttpGet("assignments")]
        public List<AssignmentStudentDto> getActiveAssignments()
        {
            return _studentAssignmentService.getActiveAssignments();
        }

        [HttpGet("assignments/history")]
        public List<AssignmentStudentDto> getAssignmentHistory()
        {
            return _studentAssignmentService.getAssignmentHistory();
        }

        [HttpPatch("flashcards/{id}/review")]
        public ActionResult reviewCard(int id, [FromBody] ReviewCardRequest request)
        {
            _flashcardsService.reviewCard(id, request);
            return Ok();
        }
    }
}
