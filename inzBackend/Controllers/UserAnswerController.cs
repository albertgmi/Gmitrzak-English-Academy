using inzBackend.Helpers;
using inzBackend.Models.AIAnswerCheckingModels;
using inzBackend.Models.ModuleReportModels;
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

        public UserAnswerController(IUserAnswerService service, IModuleReportExportService reportExportService)
        {
            _service = service;
            _reportExportService = reportExportService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<AnswerResultDto>> SubmitAnswer([FromBody] SubmitAnswerRequest request)
        {
            var result = await _service.SubmitAnswerAsync(request);
            return Ok(result);
        }

        [HttpGet("module/{moduleId}")]
        [Authorize(Roles = "User")]
        public ActionResult<List<AnswerResultDto>> GetAnswersForModule([FromRoute] int moduleId)
        {
            return Ok(_service.GetAnswersForModule(moduleId));
        }

        [HttpGet("module/{moduleId}/student/{studentId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<AnswerResultDto>> GetAnswersForStudent([FromRoute] int moduleId, [FromRoute] int studentId)
        {
            return Ok(_service.GetAnswersForModuleByStudent(moduleId, studentId));
        }

        [HttpPatch("{answerId}/override")]
        [Authorize(Roles = "Admin")]
        public ActionResult OverrideAnswer([FromRoute] int answerId, [FromBody] TeacherOverrideRequest request)
        {
            _service.OverrideAnswer(answerId, request);
            return Ok();
        }

        [HttpGet("modules/completed")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<CompletedSentenceModuleDto>> GetCompletedModules([FromQuery] int studentId, [FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);
            return Ok(_service.GetCompletedSentenceModules(studentId, from, to));
        }

        [HttpGet("report/range")]
        [Authorize(Roles = "Admin")]
        public ActionResult GenerateRangeReportPdf([FromQuery] int studentId, [FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);
            var report = _service.GenerateDateRangeReport(studentId, from, to);
            var pdf = _reportExportService.GenerateRangePdf(report);
            var filename = $"report_{report.StudentUsername}_{from}_{to}.pdf"
                .Replace(" ", "_");
            return File(pdf, "application/pdf", filename);
        }

        [HttpGet("report/range/docx")]
        [Authorize(Roles = "Admin")]
        public ActionResult GenerateRangeReportDocx([FromQuery] int studentId, [FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);
            var report = _service.GenerateDateRangeReport(studentId, from, to);
            var docx = _reportExportService.GenerateRangeDocx(report);
            var filename = $"report_{report.StudentUsername}_{from}_{to}.docx"
                .Replace(" ", "_");
            return File(docx,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                filename);
        }
        [HttpGet("report/all")]
        [Authorize(Roles = "Admin")]
        public ActionResult GetActiveStudentsReportsZip([FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);

            byte[] zipFileBytes = _reportExportService.GenerateActiveStudentsZipReport(from, to);

            var fileName = $"Reports_Active_Users_{dateFrom:yyyyMMdd}-{dateTo:yyyyMMdd}.zip";
            return File(zipFileBytes, "application/zip", fileName);
        }
    }
}