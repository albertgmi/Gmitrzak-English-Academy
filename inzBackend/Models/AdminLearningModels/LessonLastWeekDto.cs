namespace inzBackend.Models.AdminLearningModels
{
    public class LessonLastWeekDto
    {
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
        public int TotalActivityPoints { get; set; }
        public int FlashcardsStudied { get; set; }
        public int FlashcardTimeSeconds { get; set; }
        public int ListeningEpisodesWatched { get; set; }
        public List<LessonGradeDto> GradesThisWeek { get; set; } = [];
        public bool RankingCriteriaMet { get; set; }
        public int ActivityPointTarget { get; set; }
        public int FlashcardTarget { get; set; }
        public int ListeningEpisodeTarget { get; set; }
    }
}
