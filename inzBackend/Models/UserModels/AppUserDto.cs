using inzBackend.Enums;
using inzBackend.Helpers;

namespace inzBackend.Models.UserModels
{
    public class AppUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public int Streak { get; set; }
        public int? StreakOverride { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public bool IsOnline => LastActiveAt.HasValue &&
            PolandTime.DateTimeNow - LastActiveAt.Value < TimeSpan.FromMinutes(2);
    }
}
