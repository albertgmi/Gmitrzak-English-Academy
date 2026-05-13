namespace inzBackend.Models.AssignmentModels
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int MatrixId { get; set; }
        public string MatrixName { get; set; }
        public int RefreshIntervalDays { get; set; }
        public DateOnly StartDate { get; set; }
        public int CurrentModuleIndex { get; set; }
        public int TotalModules { get; set; }
    }
}
