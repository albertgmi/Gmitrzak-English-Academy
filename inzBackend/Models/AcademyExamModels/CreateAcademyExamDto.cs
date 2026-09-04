using System.ComponentModel.DataAnnotations;
using inzBackend.Enums;

namespace inzBackend.Models.AcademyExamModels
{
    public class CreateAcademyExamDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ExamLevel Level { get; set; }

        public string? MaterialsUrl { get; set; }
        public List<ExamMaterialDto> Materials { get; set; } = new();

        [Range(0, 100000)]
        public int RewardCredits { get; set; }

        [Required]
        [MaxLength(100)]
        public string PassingThreshold { get; set; } = string.Empty;

        [Required]
        public DateTime SignupDeadline { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
