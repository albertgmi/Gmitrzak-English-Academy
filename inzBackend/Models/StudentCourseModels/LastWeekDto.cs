namespace inzBackend.Models.StudentCourseModels
{
    public class LastWeekDto
    {
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
        public int TotalActivityPoints { get; set; }
        public int FlashcardsStudied { get; set; }
        public int FlashcardTimeSeconds { get; set; }
        public int ListeningEpisodesWatched { get; set; }
        public List<GradeDto> GradesThisWeek { get; set; } = [];
        public bool RankingCriteriaMet { get; set; }
    }
}
