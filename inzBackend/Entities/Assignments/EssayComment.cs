using System;
using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Assignments
{
    public class EssayComment : BaseEntity
    {
        public int UserEssayId { get; set; }
        public UserEssay UserEssay { get; set; } = null!;

        public int AuthorId { get; set; }
        public AppUser Author { get; set; } = null!;

        public string SelectedText { get; set; } = string.Empty;
        public string NoteContent { get; set; } = string.Empty;
        public string Category { get; set; } = "Grammar";
        public bool IsArchived { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
