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
        public AdminDashboardDto getAdminDashboard()
        {
            return _dashboardService.getAdminDashboard();
        }

        [HttpGet("student")]
        [Authorize(Roles = "User")]
        public StudentDashboardDto getStudentDashboard()
        {
            return _dashboardService.getStudentDashboard();
        }  
    }
}
