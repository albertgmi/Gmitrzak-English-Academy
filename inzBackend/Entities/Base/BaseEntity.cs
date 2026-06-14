using inzBackend.Helpers;

namespace inzBackend.Entities.Base
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = PolandTime.DateTimeNow;
    }
}
