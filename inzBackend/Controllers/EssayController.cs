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
        public ActionResult<EssayModuleDto> getModule(int moduleId)
        {
            return Ok(_essayService.getEssayModule(moduleId));
        }
        
        [HttpPost("submit")]
        public ActionResult<UserEssayDto> submit([FromBody] SubmitEssayRequest request)
        {
            return Ok(_essayService.submitEssay(request));
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<UserEssayDto>> getAll()
        {
            return Ok(_essayService.getAllEssaysForAdmin());
        }

        [HttpGet("admin/student/{studentId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<UserEssayDto>> getForStudent(int studentId)
        {
            return Ok(_essayService.getEssaysForStudent(studentId));
        }

        [HttpPut("admin/review/{essayId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserEssayDto> review(int essayId, [FromBody] ReviewEssayRequest request)
        {
            return Ok(_essayService.reviewEssay(essayId, request));
        }

        [HttpGet("admin/export/{essayId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult export(int essayId)
        {
            var bytes = _essayService.exportEssayToDocx(essayId);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"essay_{essayId}.docx");
        }
    }
}
