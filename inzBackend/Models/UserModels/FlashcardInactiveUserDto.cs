namespace inzBackend.Models.UserModels
{
    public class FlashcardInactiveUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastActiveAt { get; set; }
        public DateOnly? LastFlashcardStudyDate { get; set; }
        public int DaysInactive { get; set; }
        public bool IsInactiveForThreeDays { get; set; }
    }

    public class SendFlashcardRemindersRequest
    {
        public List<int> UserIds { get; set; } = new();
        public string? CustomSubject { get; set; }
        public string? CustomBody { get; set; }
    }

    public class SendRemindersResultDto
    {
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
