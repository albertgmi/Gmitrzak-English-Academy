using inzBackend.Entities.Base;

namespace inzBackend.Entities.Gamification
{
    public class ActivityPoint : BaseEntity
    {
        public int UserId { get; set; }
        public DateOnly PointDate { get; set; }
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
