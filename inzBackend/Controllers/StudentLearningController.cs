using inzBackend.Exceptions;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Models.StudentLearningModels.AlphabetModels;
using inzBackend.Models.StudentLearningModels.AssignmentStudentModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;
using inzBackend.Models.StudentLearningModels.SentenceModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Services.AiIntegrationServices;
using inzBackend.Services.StudentLearningServices.Alphabet;
using inzBackend.Services.StudentLearningServices.Assignment;
using inzBackend.Services.StudentLearningServices.Flashcards;
using inzBackend.Services.StudentLearningServices.Memories;
using inzBackend.Services.StudentLearningServices.Pronunciation;
using inzBackend.Services.StudentLearningServices.Sentences;
using inzBackend.Services.StudentLearningServices.Vocabulary;
using inzBackend.Models.StudentLearningModels.WeeklyMoviesModels;
using inzBackend.Services.StudentLearningServices.WeeklyMovies;
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
        private readonly IAlphabetService _alphabetService;
        private readonly IAiAlphabetService _aiAlphabetService;
        private readonly IWeeklyMoviesService _weeklyMoviesService;

        public StudentLearningController(ISentencesService sentencesService, IMemoriesService memoriesService,
            IPronunciationService pronunciationService, IFlashcardsService flashcardsService, IVocabularyService vocabularyService,
            IStudentAssignmentService studentAssignmentService, IAiPronunciationService aiPronunciationService,
            IAlphabetService alphabetService, IAiAlphabetService aiAlphabetService,
            IWeeklyMoviesService weeklyMoviesService)
        {
            _sentencesService = sentencesService;
            _memoriesService = memoriesService;
            _pronunciationService = pronunciationService;
            _flashcardsService = flashcardsService;
            _vocabularyService = vocabularyService;
            _studentAssignmentService = studentAssignmentService;
            _aiPronunciationService = aiPronunciationService;
            _alphabetService = alphabetService;
            _aiAlphabetService = aiAlphabetService;
            _weeklyMoviesService = weeklyMoviesService;
        }

        [HttpGet("weekly-movies")]
        [Authorize(Roles = "User,Admin")]
        public ActionResult<WeeklyMoviesResponseDto> GetWeeklyMoviesStats()
        {
            return Ok(_weeklyMoviesService.GetWeeklyMoviesStats());
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

        [HttpPut("memories/{memoryId}/add")]
        public ActionResult AddNotes([FromRoute] int memoryId, [FromBody] AddNotesRequest userNotes)
        {
            _memoriesService.AddNotes(memoryId, userNotes);
            return Ok();
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
            catch (TooManyAttemptsException ex)
            {
                return StatusCode(429, new { message = ex.Message });
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

        [HttpGet("alphabet")]
        public ActionResult<List<AlphabetEntryDto>> GetAlphabetEntries()
        {
            return _alphabetService.GetCurrentWeekEntries();
        }

        [HttpGet("alphabet/{entryId}/attempts")]
        public ActionResult<List<AlphabetAttemptDto>> GetAlphabetAttempts([FromRoute] int entryId)
        {
            return Ok(_alphabetService.GetAttempts(entryId));
        }

        [HttpPost("alphabet/generate")]
        public ActionResult GenerateAlphabetProgram()
        {
            try
            {
                _alphabetService.GenerateWeeklyProgram();
                return Ok();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("alphabet/{entryId}/attempt")]
        public async Task<ActionResult> CheckAlphabetAttempt([FromRoute] int entryId, [FromForm] IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest(new { message = "Audio file is missing or empty." });

            try
            {
                using var stream = audioFile.OpenReadStream();
                var result = await _aiAlphabetService.ProcessUserAttemptAsync(stream, audioFile.FileName, entryId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (TooManyAttemptsException ex)
            {
                return StatusCode(429, new { message = ex.Message });
            }
        }
    }
}