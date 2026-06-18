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
            var studentAssignments = _studentCourseService.getStudentsAssignments();
            return Ok(studentAssignments);
        }

        [HttpPost("complete/{matrixModuleId}")]
        public ActionResult CompleteModule([FromRoute] int matrixModuleId)
        {
            _studentCourseService.completeModule(matrixModuleId);
            return Ok();
        }

        [HttpDelete("complete/{matrixModuleId}")]
        public ActionResult UncompleteModule([FromRoute] int matrixModuleId)
        {
            _studentCourseService.uncompleteModule(matrixModuleId);
            return Ok();
        }

        [HttpGet("courses")]
        public ActionResult<List<StudentAssignmentDto>> GetCourses()
        {
            return _studentCourseService.getStudentsAssignments();
        }

        [HttpGet("last-week")]
        public ActionResult<LastWeekDto> GetLastWeek()
        {
            return _lastWeekService.getLastWeek();
        }

        [HttpGet("activity-points")]
        public ActionResult<ActivityPointsHistoryDto> GetActivityPoints()
        {
            return _activityPointService.getHistory();
        }

        [HttpGet("grades")]
        public ActionResult<List<GradeDto>> GetGrades()
        {
            return _gradesService.getGrades();
        }

        [HttpGet("stats")]
        public ActionResult<StatsDto> GetStats()
        {
            return _statsService.getStats();
        }

        [HttpGet("single-modules")]
        public ActionResult<List<StudentModuleDto>> GetSingleModules()
        {
            return _studentCourseService.getSingleModules();
        }

        [HttpPost("single-modules/complete/{id}")]
        public ActionResult CompleteSingleModule([FromRoute] int id)
        {
            _studentCourseService.completeSingleModule(id);
            return Ok();
        }

        [HttpDelete("single-modules/complete/{id}")]
        public ActionResult UncompleteSingleModule([FromRoute] int id)
        {
            _studentCourseService.uncompleteSingleModule(id);
            return Ok();
        }

        [HttpGet("completed-single-modules")]
        public ActionResult<List<StudentModuleDto>> GetCompletedSingleModules()
        {
            return _studentCourseService.getCompletedSingleModules();
        }

        [HttpGet("module/{moduleId}")]
        public ActionResult<StudentModuleDto> GetStudentModule([FromRoute] int moduleId)
        {
            var result = _studentCourseService.getStudentModule(moduleId);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("module/{moduleId}/complete")]
        public ActionResult CompleteStudentModule([FromRoute] int moduleId)
        {
            _studentCourseService.completeStudentModule(moduleId);
            return Ok();
        }
    }
}