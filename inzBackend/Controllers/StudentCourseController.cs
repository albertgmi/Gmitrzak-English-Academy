using inzBackend.Models.StudentCourseModels;
using inzBackend.Services.StudentCourseServices;
using inzBackend.Services.StudentCourseServices.ActivityPoint;
using inzBackend.Services.StudentCourseServices.Grade;
using inzBackend.Services.StudentCourseServices.LastWeek;
using inzBackend.Services.StudentCourseServices.Stats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class StudentCourseController : ControllerBase
    {
        private readonly IStudentCourseService _studentCourseService;
        private readonly ILastWeekService _lastWeekService;
        private readonly IActivityPointService _activityPointService;
        private readonly IGradesService _gradesService;
        private readonly IStatsService _statsService;

        public StudentCourseController(IStudentCourseService studentCourseService, ILastWeekService lastWeekService,
            IActivityPointService activityPointService, IGradesService gradesService, IStatsService statsService)
        {
            _studentCourseService = studentCourseService;
            _lastWeekService = lastWeekService;
            _activityPointService = activityPointService;
            _gradesService = gradesService;
            _statsService = statsService;
        }

        [HttpGet]
        public ActionResult<List<StudentAssignmentDto>> GetStudentsAssignments()
        {
            var studentAssignments = _studentCourseService.GetStudentsAssignments();
            return Ok(studentAssignments);
        }

        [HttpPost("complete/{matrixModuleId}")]
        public ActionResult CompleteModule([FromRoute] int matrixModuleId)
        {
            _studentCourseService.CompleteModule(matrixModuleId);
            return Ok();
        }

        [HttpDelete("complete/{matrixModuleId}")]
        public ActionResult UncompleteModule([FromRoute] int matrixModuleId)
        {
            _studentCourseService.UncompleteModule(matrixModuleId);
            return Ok();
        }

        [HttpGet("courses")]
        public ActionResult<List<StudentAssignmentDto>> GetCourses()
        {
            return _studentCourseService.GetStudentsAssignments();
        }

        [HttpGet("last-week")]
        public ActionResult<LastWeekDto> GetLastWeek()
        {
            return _lastWeekService.GetLastWeek();
        }

        [HttpGet("activity-points")]
        public ActionResult<ActivityPointsHistoryDto> GetActivityPoints()
        {
            return _activityPointService.GetHistory();
        }

        [HttpGet("grades")]
        public ActionResult<List<GradeDto>> GetGrades()
        {
            return _gradesService.GetGrades();
        }

        [HttpGet("stats")]
        public ActionResult<StatsDto> GetStats()
        {
            return _statsService.GetStats();
        }

        [HttpGet("single-modules")]
        public ActionResult<List<StudentModuleDto>> GetSingleModules()
        {
            return _studentCourseService.GetSingleModules();
        }

        [HttpPost("single-modules/complete/{id}")]
        public ActionResult CompleteSingleModule([FromRoute] int id)
        {
            _studentCourseService.CompleteSingleModule(id);
            return Ok();
        }

        [HttpDelete("single-modules/complete/{id}")]
        public ActionResult UncompleteSingleModule([FromRoute] int id)
        {
            _studentCourseService.UncompleteSingleModule(id);
            return Ok();
        }

        [HttpGet("completed-single-modules")]
        public ActionResult<List<StudentModuleDto>> GetCompletedSingleModules()
        {
            return _studentCourseService.GetCompletedSingleModules();
        }

        [HttpGet("module/{moduleId}")]
        public ActionResult<StudentModuleDto> GetStudentModule([FromRoute] int moduleId)
        {
            var result = _studentCourseService.GetStudentModule(moduleId);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("module/{moduleId}/complete")]
        public ActionResult CompleteStudentModule([FromRoute] int moduleId)
        {
            _studentCourseService.CompleteStudentModule(moduleId);
            return Ok();
        }
    }
}