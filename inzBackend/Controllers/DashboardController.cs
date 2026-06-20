using inzBackend.Models.DashboardModels;
using inzBackend.Services.DashboardServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult<AdminDashboardDto> GetAdminDashboard()
        {
            return _dashboardService.GetAdminDashboard();
        }

        [HttpGet("student")]
        [Authorize(Roles = "User")]
        public ActionResult<StudentDashboardDto> GetStudentDashboard()
        {
            return _dashboardService.GetStudentDashboard();
        }  
    }
}
