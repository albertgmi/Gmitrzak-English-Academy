using inzBackend.Models.DashboardModels;

namespace inzBackend.Services.DashboardServices
{
    public interface IDashboardService
    {
        AdminDashboardDto getAdminDashboard();
        StudentDashboardDto getStudentDashboard();
    }
}
