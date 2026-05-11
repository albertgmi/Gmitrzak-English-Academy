using inzBackend.Enums;

namespace inzBackend.Models.UserModels
{
    public class UpdateUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public UserRole Role { get; set; }
        public bool isActive { get; set; }

    }
}
