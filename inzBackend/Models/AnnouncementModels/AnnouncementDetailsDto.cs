namespace inzBackend.Models.AnnouncementModels
{
    public class AnnouncementDetailsDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<AnnouncementRecipientDetailsDto> Recipients { get; set; } = [];
    }
}
