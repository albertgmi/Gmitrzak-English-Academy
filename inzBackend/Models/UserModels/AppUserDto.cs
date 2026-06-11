using inzBackend.Enums;

namespace inzBackend.Models.UserModels
{
    public class AppUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public string? AvatarUrl { get; set; }
    }
}
