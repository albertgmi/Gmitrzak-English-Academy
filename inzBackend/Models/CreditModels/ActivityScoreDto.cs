namespace inzBackend.Models.CreditModels
{
    public class ActivityScoreDto
    {
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
        public int TotalScore { get; set; }
        public int HomeworkScore { get; set; }
        public int AttendanceScore { get; set; }
        public int WatchingScore { get; set; }
        public int RegularityScore { get; set; }
        public int FlashcardScore { get; set; }
        public int HomeworkDone { get; set; }
        public int HomeworkTotal { get; set; }
        public int AttendanceCount { get; set; }
        public int FlashcardDays { get; set; }
        public int FlashcardsDone { get; set; }
        public int FlashcardTarget { get; set; }
    }
}
