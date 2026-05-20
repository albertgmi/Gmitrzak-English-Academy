namespace inzBackend.Models.AnnouncementModels
{
    public class AnnouncementInboxDto
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string SenderUsername { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
    }
}
