namespace inzBackend.Models.AnnouncementModels
{
    public class CreateAnnouncementRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<int>? RecipientUserIds { get; set; }
        public string Type { get; set; } = "Announcement";
    }
}
