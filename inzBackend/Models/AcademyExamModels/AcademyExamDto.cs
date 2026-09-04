using inzBackend.Enums;

namespace inzBackend.Models.AcademyExamModels
{
    public class AcademyExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExamLevel Level { get; set; }
        public string? MaterialsUrl { get; set; }
        public int RewardCredits { get; set; }
        public string PassingThreshold { get; set; } = string.Empty;
        public DateTime SignupDeadline { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public bool IsCurrentUserSignedUp { get; set; }
        public ExamSignupStatus? CurrentUserStatus { get; set; }
        public bool CanSignUp { get; set; }
        public bool CanUnsign { get; set; }
        public int TakersCount { get; set; }
        public List<ExamTakerDto> Takers { get; set; } = new();
    }
}
