using System.ComponentModel.DataAnnotations;
using inzBackend.Enums;

namespace inzBackend.Models.AcademyExamModels
{
    public class MarkExamTakerStatusDto
    {
        [Required]
        public ExamSignupStatus Status { get; set; }
    }
}
