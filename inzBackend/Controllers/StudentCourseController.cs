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

        public StudentCourseController(
            IStudentCourseService studentCourseService,
            ILastWeekService lastWeekService,
            IActivityPointService activityPointService,
            IGradesService gradesService,
            IStatsService statsService)
        {
            _studentCourseService = studentCourseService;
            _lastWeekService = lastWeekService;
            _activityPointService = activityPointService;
            _gradesService = gradesService;
            _statsService = statsService;
        }

        [HttpGet]
        public ActionResult<List<StudentAssignmentDto>> getStudentsAssignments()
        {
            var studentAssignments = _studentCourseService.getStudentsAssignments();
            return Ok(studentAssignments);
        }

        [HttpPost("complete/{matrixModuleId}")]
        public ActionResult completeModule(int matrixModuleId)
        {
            _studentCourseService.completeModule(matrixModuleId);
            return Ok();
        }

        [HttpDelete("complete/{matrixModuleId}")]
        public ActionResult uncompleteModule(int matrixModuleId)
        {
            _studentCourseService.uncompleteModule(matrixModuleId);
            return Ok();
        }
        [HttpGet("courses")]
        public ActionResult<List<StudentAssignmentDto>> getCourses()
        {
            return _studentCourseService.getStudentsAssignments();
        }

        [HttpGet("last-week")]
        public ActionResult<LastWeekDto> getLastWeek()
        {
            return _lastWeekService.getLastWeek();
        }

        [HttpGet("activity-points")]
        public ActionResult<ActivityPointsHistoryDto> getActivityPoints()
        {
            return _activityPointService.getHistory();
        }

        [HttpGet("grades")]
        public ActionResult<List<GradeDto>> getGrades()
        {
            return _gradesService.getGrades();
        }

        [HttpGet("stats")]
        public ActionResult<StatsDto> getStats()
        {
            return _statsService.getStats();
        }

        [HttpGet("single-modules")]
        public ActionResult<List<StudentModuleDto>> GetSingleModules()
        {
            return _studentCourseService.getSingleModules();
        }

        [HttpPost("single-modules/complete/{id}")]
        public ActionResult CompleteSingleModule(int id)
        {
            _studentCourseService.completeSingleModule(id);
            return Ok();
        }

        [HttpDelete("single-modules/complete/{id}")]
        public ActionResult UncompleteSingleModule(int id)
        {
            _studentCourseService.uncompleteSingleModule(id);
            return Ok();
        }

        [HttpGet("completed-single-modules")]
        public ActionResult<List<StudentModuleDto>> getCompletedSingleModules()
        {
            return _studentCourseService.getCompletedSingleModules();
        }
    }
}
