using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices.Stats
{
    public interface IStatsService
    {
        StatsDto GetStats();
    }
}