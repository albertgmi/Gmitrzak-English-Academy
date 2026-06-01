using inzBackend.Entities;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Services.GlobalVocabularyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/vocabulary")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GlobalVocabularyController : ControllerBase
    {
        private readonly IGlobalVocabularyService _globalVocabularyService;

        public GlobalVocabularyController(IGlobalVocabularyService glboalVocabularyService)
        {
            _globalVocabularyService = glboalVocabularyService;
        }

        [HttpGet]
        public ActionResult<List<GlobalVocabularyDto>> getAllVocabulary()
        {
            return Ok(_globalVocabularyService.getAllVocabulary());
        }

        [HttpPost]
        public ActionResult<Vocabulary> createVocabulary([FromBody] VocabularyAddingRequest request)
        {
            var newVocabulary = _globalVocabularyService.createNewVocabulary(request);
            return Created();
        }

        [HttpPut("update/{vocabularyId}")]
        public ActionResult updateVocabulary([FromBody] VocabularyUpdateRequest request, [FromRoute] int vocabularyId)
        {
            _globalVocabularyService.updateVocabulary(request, vocabularyId);
            return Ok();
        }

        [HttpGet("search")]
        public ActionResult<SearchVocabularyResult> searchVocabulary([FromQuery] string query, [FromQuery] int studentUserId)
        {
            var result = _globalVocabularyService.searchVocabulary(query, studentUserId);
            return Ok(result);
        }

        [HttpPost("translation")]
        public ActionResult<VocabularyDto> addTranslation([FromBody] AddTranslationRequest request)
        {
            var result = _globalVocabularyService.addTranslation(request);
            return Ok(result);
        }

        [HttpPost("assign")]
        public ActionResult assignVocabularyToStudent([FromBody] AssignVocabularyToStudentRequest request)
        {
            _globalVocabularyService.assignVocabularyToStudent(request);
            return Ok();
        }

        [HttpPost("assign-multiple")]
        public ActionResult AssignMultiple([FromBody] AssignMultipleVocabularyToStudentRequest request)
        {
            _globalVocabularyService.assignMultipleVocabularyToStudent(request);
            return Ok();
        }
    }
}
