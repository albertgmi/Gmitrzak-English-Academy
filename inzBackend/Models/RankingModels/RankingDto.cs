namespace inzBackend.Models.RankingModels
{
    public class RankingDto
    {
        public string Period { get; set; } = string.Empty;
        public List<RankingEntryDto> Entries { get; set; } = [];
        public int CurrentUserPosition { get; set; }
        public int PointsToNextPosition { get; set; }
        public bool CurrentUserOnPodium { get; set; }
    }
}
