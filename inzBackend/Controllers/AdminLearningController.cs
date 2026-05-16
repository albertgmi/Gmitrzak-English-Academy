using inzBackend.Models.AdminLearningModels;
using inzBackend.Services.AdminFlashcardServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/learning")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminLearningController : ControllerBase
    {
        private readonly IAdminFlashcardService _adminFlashcardService;

        public AdminLearningController(IAdminFlashcardService adminFlashcardService)
        {
            _adminFlashcardService = adminFlashcardService;
        }

        [HttpGet]
        public ActionResult<List<AdminStudentStudySummaryDto>> getAdminStudyLogsSummary()
        {
            return Ok(_adminFlashcardService.getAdminStudyLogsSummary());
        }
    }
}
