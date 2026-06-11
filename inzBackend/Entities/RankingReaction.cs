using inzBackend.Models;

namespace inzBackend.Entities
{
    public class RankingReaction : BaseEntity
    {
        public int FromUserId { get; set; }
        public AppUser FromUser { get; set; } = null!;
        public int ToUserId { get; set; }
        public AppUser ToUser { get; set; } = null!;
        public string Emoji { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public DateOnly ReactionDate { get; set; }
    }
}
