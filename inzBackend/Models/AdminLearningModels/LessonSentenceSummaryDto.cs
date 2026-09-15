using inzBackend.Models.StudentLearningModels.SentenceModels;

namespace inzBackend.Models.AdminLearningModels
{
    public class LessonSentenceSummaryDto
    {
        public int TotalCards { get; set; }
        public int LeechCount { get; set; }
        public int StudiedTodayCount { get; set; }
        public int DueCount { get; set; }
        public List<SentenceDto> Leeches { get; set; } = [];
        public List<SentenceDto> StudiedToday { get; set; } = [];
    }
}
