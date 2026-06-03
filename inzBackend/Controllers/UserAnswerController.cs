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
        public async Task<ActionResult<AnswerResultDto>> submitAnswer([FromBody] SubmitAnswerRequest request)
        {
            var result = await _service.submitAnswerAsync(request);
            return Ok(result);
        }

        [HttpGet("module/{moduleId}")]
        [Authorize(Roles = "User")]
        public ActionResult<List<AnswerResultDto>> getAnswersForModule([FromRoute] int moduleId)
        {
            return Ok(_service.getAnswersForModule(moduleId));
        }

        [HttpGet("module/{moduleId}/student/{studentId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<AnswerResultDto>> getAnswersForStudent([FromRoute] int moduleId, [FromRoute] int studentId)
        {
            return Ok(_service.getAnswersForModuleByStudent(moduleId, studentId));
        }

        [HttpPatch("{answerId}/override")]
        [Authorize(Roles = "Admin")]
        public ActionResult overrideAnswer([FromRoute] int answerId, [FromBody] TeacherOverrideRequest request)
        {
            _service.overrideAnswer(answerId, request);
            return Ok();
        }

        [HttpGet("modules/completed")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<CompletedSentenceModuleDto>> getCompletedModules([FromQuery] int studentId, [FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);
            return Ok(_service.getCompletedSentenceModules(studentId, from, to));
        }

        [HttpGet("report/range")]
        [Authorize(Roles = "Admin")]
        public ActionResult generateRangeReportPdf([FromQuery] int studentId, [FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);
            var report = _service.generateDateRangeReport(studentId, from, to);
            var pdf = _reportExportService.generateRangePdf(report);
            var filename = $"report_{report.StudentUsername}_{from}_{to}.pdf"
                .Replace(" ", "_");
            return File(pdf, "application/pdf", filename);
        }

        [HttpGet("report/range/docx")]
        [Authorize(Roles = "Admin")]
        public ActionResult generateRangeReportDocx([FromQuery] int studentId, [FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);
            var report = _service.generateDateRangeReport(studentId, from, to);
            var docx = _reportExportService.generateRangeDocx(report);
            var filename = $"report_{report.StudentUsername}_{from}_{to}.docx"
                .Replace(" ", "_");
            return File(docx,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                filename);
        }
        [HttpGet("report/all")]
        [Authorize(Roles = "Admin")]
        public ActionResult getActiveStudentsReportsZip([FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            var from = PolandTime.ParseDate(dateFrom);
            var to = PolandTime.ParseDate(dateTo);

            byte[] zipFileBytes = _reportExportService.generateActiveStudentsZipReport(from, to);

            var fileName = $"Reports_Active_Users_{dateFrom:yyyyMMdd}-{dateTo:yyyyMMdd}.zip";
            return File(zipFileBytes, "application/zip", fileName);
        }
    }
}