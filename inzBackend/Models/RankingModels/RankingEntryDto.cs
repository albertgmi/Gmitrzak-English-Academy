using inzBackend.Helpers;

namespace inzBackend.Models.RankingModels
{
    public class RankingEntryDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public bool IsOnline => LastActiveAt.HasValue &&
            PolandTime.DateTimeNow - LastActiveAt.Value < TimeSpan.FromMinutes(2);
        public int Position { get; set; }
        public int ActivityPoints { get; set; }
        public decimal AverageGrade { get; set; }
        public int FlashcardsDone { get; set; }
        public int Streak { get; set; }
        public int Score { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PositionChange { get; set; }
        public Dictionary<string, int> Reactions { get; set; } = new();
    }
}
