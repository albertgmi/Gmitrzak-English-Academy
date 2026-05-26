namespace inzBackend.Models.RankingModels
{
    public class AddReactionRequest
    {
        public int ToUserId { get; set; }
        public string Emoji { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
    }
}
