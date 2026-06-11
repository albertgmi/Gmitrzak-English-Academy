namespace inzBackend.Models.StudentLearningModels.FlashcardModels
{
    public class FlashcardStudyLogDto
    {
        public int Id { get; set; }
        public DateOnly StudyDate { get; set; }
        public int EasyCount { get; set; }
        public int HardCount { get; set; }
        public int IncorrectCount { get; set; }
        public int TimeSpentSeconds { get; set; }
        public string FlashcardFront { get; set; }
    }
}
