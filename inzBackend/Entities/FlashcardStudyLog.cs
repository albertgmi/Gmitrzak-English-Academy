namespace inzBackend.Models
{
    public class FlashcardStudyLog : BaseEntity
    {
        public int FlashcardId { get; set; }
        public virtual Flashcard Flashcard { get; set; } = null!;
        public int UserId { get; set; }
        public DateOnly StudyDate { get; set; }
        public int EasyCount { get; set; } = 0;
        public int HardCount { get; set; } = 0;
        public int IncorrectCount { get; set; } = 0;
        public int TimeSpentSeconds { get; set; } = 0;
    }
}
