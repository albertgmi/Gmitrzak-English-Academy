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
        public string Type { get; set; } = string.Empty;
        public int SignUpCount { get; set; }
        public int VoteYesCount { get; set; }
        public int VoteNoCount { get; set; }
    }
}
