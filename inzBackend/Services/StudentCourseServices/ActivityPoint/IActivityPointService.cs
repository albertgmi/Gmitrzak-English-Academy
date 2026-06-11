using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices.ActivityPoint
{
    public interface IActivityPointService
    {
        ActivityPointsHistoryDto getHistory();
    }
}