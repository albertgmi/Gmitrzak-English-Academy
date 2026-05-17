namespace inzBackend.Models.AdminLearningModels
{
    public class LessonFlashcardSummaryDto
    {
        public int TotalCards { get; set; }
        public int LeechCount { get; set; }
        public int StudiedTodayCount { get; set; }
        public int DueCount { get; set; }
        public List<LessonFlashcardDto> Leeches { get; set; } = [];
        public List<LessonFlashcardDto> StudiedToday { get; set; } = [];
        public List<LessonStudyLogDto> RecentLogs { get; set; } = [];
    }
}
