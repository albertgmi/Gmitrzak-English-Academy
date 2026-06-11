namespace inzBackend.Models.EssayModels
{
    public class UserEssayDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string EssayPrompt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? AdminContent { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsReviewed { get; set; }
        public DateOnly? SubmittedDate { get; set; }
        public DateOnly? ReviewedDate { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
