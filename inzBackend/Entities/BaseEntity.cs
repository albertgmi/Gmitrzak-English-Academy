using inzBackend.Helpers;

namespace inzBackend.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = PolandTime.DateTimeNow;
    }
}
