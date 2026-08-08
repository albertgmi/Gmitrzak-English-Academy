namespace inzBackend.Models.StudentLearningModels.WeeklyMoviesModels
{
    public class WeeklyMovieItemDto
    {
        public int Rank { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalWatchedCount { get; set; }
        public int UniqueViewersCount { get; set; }
    }

    public class TopWatcherDto
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int TotalWatchedCount { get; set; }
    }

    public class WeeklyMoviesResponseDto
    {
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public int TotalEpisodesWatched { get; set; }
        public List<WeeklyMovieItemDto> TopMovies { get; set; } = new();
        public List<TopWatcherDto> TopWatchers { get; set; } = new();
    }
}
