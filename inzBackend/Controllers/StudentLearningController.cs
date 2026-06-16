using inzBackend.Exceptions;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Models.StudentLearningModels.AssignmentStudentModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;
using inzBackend.Models.StudentLearningModels.SentenceModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Services.AiIntegrationServices;
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
        private readonly IAiPronunciationService _aiPronunciationService;

        public StudentLearningController(ISentencesService sentencesService, IMemoriesService memoriesService,
            IPronunciationService pronunciationService, IFlashcardsService flashcardsService, IVocabularyService vocabularyService,
            IStudentAssignmentService studentAssignmentService, IAiPronunciationService aiPronunciationService)
        {
            _sentencesService = sentencesService;
            _memoriesService = memoriesService;
            _pronunciationService = pronunciationService;
            _flashcardsService = flashcardsService;
            _vocabularyService = vocabularyService;
            _studentAssignmentService = studentAssignmentService;
            _aiPronunciationService = aiPronunciationService;
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

        [HttpGet("pronunciation/correct")]
        public ActionResult<List<PronunciationTestItemDto>> getCorrectPronunciation()
        {
            return _pronunciationService.getCorrectPronunciation();
        }

        [HttpGet("pronunciation/{entryId}/attempts")]
        public ActionResult<List<PronunciationAttemptDto>> getAttempts([FromRoute] int entryId)
        {
            var attempts = _pronunciationService.getAttemptsAsync(entryId);
            return Ok(attempts);
        }

        [HttpPost("pronunciation/{entryId}/attempt")]
        public async Task<ActionResult> checkPronunciation([FromRoute] int entryId, [FromForm] IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest(new { message = "Audio file is missing or empty." });

            try
            {
                using var stream = audioFile.OpenReadStream();
                var result = await _aiPronunciationService.processUserAttemptAsync(stream, audioFile.FileName, entryId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
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
        public ActionResult<List<FlashcardDto>> searchFlashcards([FromQuery] string query)
        {
            return _flashcardsService.searchFlashcards(query);
        }

        [HttpGet("vocabulary")]
        public ActionResult<List<VocabularyDto>> getAllVocabulary()
        {
            return _vocabularyService.getAllVocabulary();
        }

        [HttpGet("assignments")]
        public ActionResult<List<AssignmentStudentDto>> getActiveAssignments()
        {
            return _studentAssignmentService.getActiveAssignments();
        }

        [HttpGet("assignments/history")]
        public ActionResult<List<AssignmentStudentDto>> getAssignmentHistory()
        {
            return _studentAssignmentService.getAssignmentHistory();
        }

        [HttpPatch("flashcards/{id}/review")]
        public ActionResult reviewCard([FromRoute] int id, [FromBody] ReviewCardRequest request)
        {
            _flashcardsService.reviewCard(id, request);
            return Ok();
        }

        [HttpGet("module/{moduleId}/sentences")]
        public ActionResult<ModuleSentenceSessionDto> getModuleSentences([FromRoute] int moduleId)
        {
            return _sentencesService.getModuleSentences(moduleId);
        }

        [HttpPatch("sentences/{id}/review")]
        public ActionResult reviewSentence([FromRoute] int id, [FromBody] ReviewSentenceRequest request)
        {
            _sentencesService.reviewSentence(id, request);
            return Ok();
        }
    }
}