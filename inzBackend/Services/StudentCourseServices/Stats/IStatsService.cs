using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices
{
    public interface IStatsService
    {
        StatsDto getStats();
    }
}