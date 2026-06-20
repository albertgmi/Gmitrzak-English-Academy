using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices.LastWeek
{
    public interface ILastWeekService
    {
        LastWeekDto GetLastWeek();
    }
}