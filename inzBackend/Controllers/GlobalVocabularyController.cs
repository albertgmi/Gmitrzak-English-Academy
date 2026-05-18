using inzBackend.Entities;
using inzBackend.Models.GlobalVocabularyModels;
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
    }
}
