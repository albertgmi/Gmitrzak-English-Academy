using System;
using inzBackend.Entities.Base;
namespace inzBackend.Entities.Assignments
{
    public class UserSentenceAnswerComment : AuditableEntity
    {
        public int UserSentenceAnswerId { get; set; }
        public UserSentenceAnswer UserSentenceAnswer { get; set; } = null!;
        public string SelectedText { get; set; } = string.Empty;
        public string NoteContent { get; set; } = string.Empty;
        public string Category { get; set; } = "Grammar"; 
        public string Author { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsArchived { get; set; } = false;
    }
}
