using inzBackend.Entities.LearningMaterials;
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
        public ActionResult<List<GlobalVocabularyDto>> GetAllVocabulary()
        {
            return Ok(_globalVocabularyService.GetAllVocabulary());
        }

        [HttpPost]
        public ActionResult<Vocabulary> CreateVocabulary([FromBody] VocabularyAddingRequest request)
        {
            var newVocabulary = _globalVocabularyService.CreateNewVocabulary(request);
            return Created();
        }

        [HttpPut("update/{vocabularyId}")]
        public ActionResult UpdateVocabulary([FromBody] VocabularyUpdateRequest request, [FromRoute] int vocabularyId)
        {
            _globalVocabularyService.UpdateVocabulary(request, vocabularyId);
            return Ok();
        }

        [HttpGet("search")]
        public async Task<ActionResult<SearchVocabularyResult>> SearchVocabulary([FromQuery] string query, [FromQuery] int studentUserId)
        {
            var result = await _globalVocabularyService.SearchVocabulary(query, studentUserId);
            return Ok(result);
        }

        [HttpPost("translation")]
        public ActionResult<VocabularyDto> AddTranslation([FromBody] AddTranslationRequest request)
        {
            var result = _globalVocabularyService.AddTranslation(request);
            return Ok(result);
        }

        [HttpPost("assign")]
        public ActionResult AssignVocabularyToStudent([FromBody] AssignVocabularyToStudentRequest request)
        {
            _globalVocabularyService.AssignVocabularyToStudent(request);
            return Ok();
        }

        [HttpPost("assign-multiple")]
        public ActionResult AssignMultiple([FromBody] AssignMultipleVocabularyToStudentRequest request)
        {
            _globalVocabularyService.AssignMultipleVocabularyToStudent(request);
            return Ok();
        }

        [HttpPost("assign-catalogue")]
        public ActionResult AssignCatalogue([FromBody] AssignCatalogueToStudentRequest request)
        {
            _globalVocabularyService.AssignCatalogueToStudent(request);
            return Ok();
        }
    }
}
