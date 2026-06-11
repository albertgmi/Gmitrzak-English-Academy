namespace inzBackend.Models.AdminLearningModels
{
    public class DailyStudyTimeDto
    {
        public DateOnly StudyDate { get; set; }
        public int TimeSpentSeconds { get; set; }
        public int FlashcardsDone { get; set; }
        public int EasyCount { get; set; }
        public int HardCount { get; set; }
        public int IncorrectCount { get; set; }
    }
}
