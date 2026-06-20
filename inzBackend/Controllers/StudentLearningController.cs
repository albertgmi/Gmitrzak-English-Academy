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
        public ActionResult<List<SentenceDto>> GetAllSentences()
        {
            return _sentencesService.GetAllSentences();
        }

        [HttpGet("memories")]
        public ActionResult<List<MemoryDto>> GetAllMemories()
        {
            return _memoriesService.GetAllMemories();
        }

        [HttpGet("pronunciation")]
        public ActionResult<List<PronunciationEntryDto>> GetAllPronunciation()
        {
            return _pronunciationService.GetAllEntries();
        }

        [HttpGet("pronunciation/correct")]
        public ActionResult<List<PronunciationTestItemDto>> GetCorrectPronunciation()
        {
            return _pronunciationService.GetCorrectPronunciation();
        }

        [HttpGet("pronunciation/{entryId}/attempts")]
        public ActionResult<List<PronunciationAttemptDto>> GetAttempts([FromRoute] int entryId)
        {
            var attempts = _pronunciationService.GetAttempts(entryId);
            return Ok(attempts);
        }

        [HttpPost("pronunciation/{entryId}/attempt")]
        public async Task<ActionResult> CheckPronunciation([FromRoute] int entryId, [FromForm] IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest(new { message = "Audio file is missing or empty." });

            try
            {
                using var stream = audioFile.OpenReadStream();
                var result = await _aiPronunciationService.ProcessUserAttemptAsync(stream, audioFile.FileName, entryId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("flashcards")]
        public ActionResult<List<FlashcardDto>> GetAllFlashcards()
        {
            return _flashcardsService.GetAllFlashcards();
        }

        [HttpGet("flashcards/leeches")]
        public ActionResult<List<FlashcardDto>> GetLeeches()
        {
            return _flashcardsService.GetLeeches();
        }

        [HttpGet("flashcards/studied-today")]
        public ActionResult<List<FlashcardDto>> GetStudiedToday()
        {
            return _flashcardsService.GetStudiedToday();
        }

        [HttpGet("flashcards/logs")]
        public ActionResult<List<FlashcardStudyLogDto>> GetStudyLogs()
        {
            return _flashcardsService.GetStudyLogs();
        }

        [HttpGet("flashcards/search")]
        public ActionResult<List<FlashcardDto>> SearchFlashcards([FromQuery] string query)
        {
            return _flashcardsService.SearchFlashcards(query);
        }

        [HttpGet("vocabulary")]
        public ActionResult<List<VocabularyDto>> GetAllVocabulary()
        {
            return _vocabularyService.GetAllVocabulary();
        }

        [HttpGet("assignments")]
        public ActionResult<List<AssignmentStudentDto>> GetActiveAssignments()
        {
            return _studentAssignmentService.GetActiveAssignments();
        }

        [HttpGet("assignments/history")]
        public ActionResult<List<AssignmentStudentDto>> GetAssignmentHistory()
        {
            return _studentAssignmentService.GetAssignmentHistory();
        }

        [HttpPatch("flashcards/{id}/review")]
        public ActionResult ReviewCard([FromRoute] int id, [FromBody] ReviewCardRequest request)
        {
            _flashcardsService.ReviewCard(id, request);
            return Ok();
        }

        [HttpGet("module/{moduleId}/sentences")]
        public ActionResult<ModuleSentenceSessionDto> GetModuleSentences([FromRoute] int moduleId)
        {
            return _sentencesService.GetModuleSentences(moduleId);
        }

        [HttpPatch("sentences/{id}/review")]
        public ActionResult ReviewSentence([FromRoute] int id, [FromBody] ReviewSentenceRequest request)
        {
            _sentencesService.ReviewSentence(id, request);
            return Ok();
        }
    }
}