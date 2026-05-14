using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices
{
    public interface IActivityPointService
    {
        ActivityPointsHistoryDto getHistory();
    }
}