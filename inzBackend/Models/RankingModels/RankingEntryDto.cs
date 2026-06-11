namespace inzBackend.Models.RankingModels
{
    public class RankingEntryDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int Position { get; set; }
        public int ActivityPoints { get; set; }
        public decimal AverageGrade { get; set; }
        public int FlashcardsDone { get; set; }
        public int Score { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PositionChange { get; set; }
        public Dictionary<string, int> Reactions { get; set; } = new();
    }
}
