using inzBackend.Models;

namespace inzBackend.Entities
{
    public class Agenda : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int ActivityPointTarget { get; set; } = 500;
        public int FlashcardTarget { get; set; } = 50;
        public int ListeningEpisodeTarget { get; set; } = 1;
        public string Notes { get; set; } = string.Empty;
    }
}
