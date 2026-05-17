namespace inzBackend.Models.AdminLearningModels
{
    public class StudentStudyTimeDto
    {
        public int TotalTimeSpentSeconds { get; set; }
        public int TotalFlashcardsDone { get; set; }
        public int EasyCount { get; set; }
        public int HardCount { get; set; }
        public int IncorrectCount { get; set; }
        public List<DailyStudyTimeDto> DailyBreakdown { get; set; } = [];
    }
}
