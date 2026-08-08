using inzBackend.Models.StudentLearningModels.WeeklyMoviesModels;

namespace inzBackend.Services.StudentLearningServices.WeeklyMovies
{
    public interface IWeeklyMoviesService
    {
        WeeklyMoviesResponseDto GetWeeklyMoviesStats();
    }
}
