namespace inzBackend.Models.AnnouncementModels
{
    public class AnnouncementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string SenderUsername { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public int TotalRecipients { get; set; }
        public int ReadCount { get; set; }
    }
}
