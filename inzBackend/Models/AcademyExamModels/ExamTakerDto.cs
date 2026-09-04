using inzBackend.Enums;

namespace inzBackend.Models.AcademyExamModels
{
    public class ExamTakerDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime SignedUpAt { get; set; }
        public ExamSignupStatus Status { get; set; }
    }
}
