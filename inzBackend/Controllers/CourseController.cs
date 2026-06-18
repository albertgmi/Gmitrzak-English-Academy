using inzBackend.Entities.Curriculum;
using inzBackend.Models.CourseModels;
using inzBackend.Services.CourseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/course")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<CourseDto>> GetAllCourses()
        {
            return _courseService.getAllCourses();
        }

        [HttpPost]
        public ActionResult<Course> CreateCourse(CreateCourseRequest request)
        {
            return _courseService.createCourse(request);
        }

        [HttpPut("{courseId}")]
        public ActionResult UpdateCourse([FromRoute] int courseId, [FromBody] UpdateCourseRequest request)
        {
            _courseService.updateCourse(courseId, request);
            return Ok();
        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourse([FromRoute] int courseId)
        {
            _courseService.deleteCourse(courseId);
            return NoContent();
        }

        [HttpPost("{courseId}/programs/{programId}")]
        public ActionResult AssignProgram([FromRoute] int courseId, [FromRoute] int programId)
        {
            _courseService.assignProgram(courseId, programId);
            return Ok();
        }

        [HttpDelete("{courseId}/programs/{programId}")]
        public ActionResult RemoveProgram([FromRoute] int courseId, [FromRoute] int programId)
        {
            _courseService.removeProgram(courseId, programId);
            return Ok();
        }
    }
}
