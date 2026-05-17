namespace inzBackend.Models.DashboardModels
{
    public class AdminDashboardDto
    {
        public int TotalStudents { get; set; }
        public int ActiveStudentsThisWeek { get; set; }
        public int TotalFlashcards { get; set; }
        public int TotalAssignmentsPending { get; set; }
        public List<RecentGradeDto> RecentGrades { get; set; } = [];
        public List<UpcomingAssignmentDto> UpcomingAssignments { get; set; } = [];
        public List<StudentPointsDto> TopStudentsByPoints { get; set; } = [];
    }
}
