namespace inzBackend.Models.AnnouncementModels
{
    public class AnnouncementRecipientDetailsDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        public bool? SignedUp { get; set; }
        public bool? Vote { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
