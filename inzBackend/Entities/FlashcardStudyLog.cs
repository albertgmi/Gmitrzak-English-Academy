namespace inzBackend.Models
{
    public class FlashcardStudyLog : BaseEntity
    {
        public int FlashcardId { get; set; }
        public int UserId { get; set; }
        public DateOnly StudyDate { get; set; }
        public int EasyCount { get; set; }
        public int HardCount { get; set; }
        public int IncorrectCount { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}
