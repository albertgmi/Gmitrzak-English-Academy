using System;

namespace inzBackend.Models.LiveNotepadModels
{
    public class LiveNoteSummaryDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentUsername { get; set; } = string.Empty;
        public string? StudentAvatarUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PreviewText { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
    }

    public class LiveNoteDetailDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentUsername { get; set; } = string.Empty;
        public string? StudentAvatarUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
    }

    public class CreateLiveNoteRequest
    {
        public string Title { get; set; } = string.Empty;
        public int? StudentId { get; set; } // If admin, specifies student. If student, defaults to current user.
    }

    public class SaveLiveNoteRequest
    {
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class LiveNoteCollaborativeUserDto
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
