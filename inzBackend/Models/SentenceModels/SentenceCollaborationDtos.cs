using System;

namespace inzBackend.Models.SentenceModels
{
    public class SentenceCollaborativeUserDto
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class SentenceModuleLiveDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentUsername { get; set; } = string.Empty;
        public string? StudentAvatarUrl { get; set; }
        public int TotalSentences { get; set; }
        public int AnsweredSentences { get; set; }
        public int CorrectCount { get; set; }
        public int PartialCount { get; set; }
        public int IncorrectCount { get; set; }
        public bool IsReviewed { get; set; }
        public DateTime? LastAnswerDate { get; set; }
    }

    public class SentenceAnswerLiveDto
    {
        public int Id { get; set; } // UserSentenceAnswerId
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int SentenceStockId { get; set; }
        public string Polish { get; set; } = string.Empty;
        public string ExpectedTranslation { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string? AdminCorrection { get; set; }
        public string AiResult { get; set; } = string.Empty;
        public string AiExplanation { get; set; } = string.Empty;
        public string? TeacherOverride { get; set; }
        public string? TeacherExplanation { get; set; }
        public bool TeacherReviewed { get; set; }
        public string StudentUsername { get; set; } = string.Empty;
        public string? StudentAvatarUrl { get; set; }
    }

    public class SaveSentenceReviewRequest
    {
        public string? AdminCorrection { get; set; }
        public string? TeacherOverride { get; set; }
        public string? TeacherExplanation { get; set; }
    }

    public class SentenceAnswerCommentDto
    {
        public int Id { get; set; }
        public string NoteId => $"note_{Id}";
        public int UserSentenceAnswerId { get; set; }
        public string SelectedText { get; set; } = string.Empty;
        public string NoteContent { get; set; } = string.Empty;
        public string Category { get; set; } = "Grammar";
        public string Author { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsArchived { get; set; }
    }

    public class CreateSentenceAnswerCommentRequest
    {
        public string SelectedText { get; set; } = string.Empty;
        public string NoteContent { get; set; } = string.Empty;
        public string Category { get; set; } = "Grammar";
    }
}
