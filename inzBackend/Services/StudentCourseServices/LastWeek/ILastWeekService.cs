using inzBackend.Models.StudentCourseModels;

namespace inzBackend.Services.StudentCourseServices
{
    public interface ILastWeekService
    {
        LastWeekDto getLastWeek();
    }
}