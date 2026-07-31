using inzBackend.Helpers;

namespace inzBackend.Models.UserModels
{
    public class StudentActivityDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastActiveAt { get; set; }

        public bool IsOnline => LastActiveAt.HasValue &&
            PolandTime.DateTimeNow - LastActiveAt.Value < TimeSpan.FromMinutes(2);
    }
}
