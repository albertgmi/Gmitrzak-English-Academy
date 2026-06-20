using inzBackend.Models.ExaminationModels;
using inzBackend.Services.ExaminationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/examination")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ExaminationController : ControllerBase
    {
        private readonly IExaminationService _examinationService;
        public ExaminationController(IExaminationService examinationService)
        {
            _examinationService = examinationService;
        }

        [HttpGet("{studentId}")]
        public ActionResult<ExaminationDto> GetExamination(int studentId)
        {
            return _examinationService.GetExamination(studentId);
        }
    }
}
