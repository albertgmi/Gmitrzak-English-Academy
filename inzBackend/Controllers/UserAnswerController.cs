using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Services.ReportServices;
using inzBackend.Services.UserAnswerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/answers")]
    [ApiController]
    [Authorize]
    public class UserAnswerController : ControllerBase
    {
        private readonly IUserAnswerService _service;
        private readonly IModuleReportExportService _reportExportService;

        public UserAnswerController(
            IUserAnswerService service,
            IModuleReportExportService reportExportService)
        {
            _service = service;
            _reportExportService = reportExportService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<AnswerResultDto>> submitAnswer(
            [FromBody] SubmitAnswerRequest request)
        {
            var result = await _service.submitAnswerAsync(request);
            return Ok(result);
        }

        [HttpGet("module/{moduleId}")]
        [Authorize(Roles = "User")]
        public ActionResult<List<AnswerResultDto>> getAnswersForModule(int moduleId)
        {
            return Ok(_service.getAnswersForModule(moduleId));
        }

        [HttpGet("module/{moduleId}/student/{studentId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<AnswerResultDto>> getAnswersForStudent(
            int moduleId,
            int studentId)
        {
            return Ok(_service.getAnswersForModuleByStudent(moduleId, studentId));
        }

        [HttpPatch("{answerId}/override")]
        [Authorize(Roles = "Admin")]
        public IActionResult overrideAnswer(
            int answerId,
            [FromBody] TeacherOverrideRequest request)
        {
            _service.overrideAnswer(answerId, request);
            return Ok();
        }

        [HttpGet("module/{moduleId}/student/{studentId}/report/pdf")]
        [Authorize(Roles = "Admin")]
        public IActionResult generatePdfReport(
            int moduleId,
            int studentId)
        {
            var report = _service.generateReport(moduleId, studentId);

            var pdf = _reportExportService.GeneratePdf(report);

            var filename =
                $"report_{report.StudentUsername}_{report.ModuleName}_{report.GeneratedDate}.pdf"
                .Replace(" ", "_");

            return File(pdf, "application/pdf", filename);
        }

        [HttpGet("module/{moduleId}/student/{studentId}/report/docx")]
        [Authorize(Roles = "Admin")]
        public IActionResult generateDocxReport(
            int moduleId,
            int studentId)
        {
            var report = _service.generateReport(moduleId, studentId);

            var docx = _reportExportService.GenerateDocx(report);

            var filename =
                $"report_{report.StudentUsername}_{report.ModuleName}_{report.GeneratedDate}.docx"
                .Replace(" ", "_");

            return File(
                docx,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                filename);
        }
    }
}