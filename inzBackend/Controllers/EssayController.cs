using inzBackend.Models.EssayModels;
using inzBackend.Services.EssayServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/essay")]
    [ApiController]
    [Authorize]
    public class EssayController : ControllerBase
    {
        private readonly IEssayService _essayService;

        public EssayController(IEssayService essayService)
        {
            _essayService = essayService;
        }

        [HttpGet("module/{moduleId}")]
        public ActionResult<EssayModuleDto> GetModule(int moduleId)
        {
            return Ok(_essayService.GetEssayModule(moduleId));
        }
        
        [HttpPost("submit")]
        public ActionResult<UserEssayDto> Submit([FromBody] SubmitEssayRequest request)
        {
            return Ok(_essayService.SubmitEssay(request));
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<UserEssayDto>> GetAll()
        {
            return Ok(_essayService.GetAllEssaysForAdmin());
        }

        [HttpGet("admin/student/{studentId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<UserEssayDto>> GetForStudent(int studentId)
        {
            return Ok(_essayService.GetEssaysForStudent(studentId));
        }

        [HttpPut("admin/review/{essayId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserEssayDto> Review(int essayId, [FromBody] ReviewEssayRequest request)
        {
            return Ok(_essayService.ReviewEssay(essayId, request));
        }

        [HttpGet("admin/export/{essayId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Export(int essayId)
        {
            var bytes = _essayService.ExportEssayToDocx(essayId);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"essay_{essayId}.docx");
        }
    }
}
