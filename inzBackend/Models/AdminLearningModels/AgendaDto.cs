namespace inzBackend.Models.AdminLearningModels
{
    public class AgendaDto
    {
        public int Id { get; set; }
        public int ActivityPointTarget { get; set; }
        public int FlashcardTarget { get; set; }
        public int ListeningEpisodeTarget { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
