namespace inzBackend.Models.AdminFlashcardModels
{
    public class AdminStudentStudySummaryDto
    {
        public string Username { get; set; }
        public int? TotalTimeSpentSeconds { get; set; }
        public int? TotalFlashcardsDone { get; set; }
    }
}
