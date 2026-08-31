using System;

namespace inzBackend.Models.EssayModels
{
    public class EssayCommentDto
    {
        public int Id { get; set; }
        public string NoteId { get; set; } = string.Empty;
        public int UserEssayId { get; set; }
        public int AuthorId { get; set; }
        public string Author { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string SelectedText { get; set; } = string.Empty;
        public string NoteContent { get; set; } = string.Empty;
        public string Category { get; set; } = "Grammar";
        public bool IsArchived { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class CreateEssayCommentRequest
    {
        public string SelectedText { get; set; } = string.Empty;
        public string NoteContent { get; set; } = string.Empty;
        public string Category { get; set; } = "Grammar";
    }
}
