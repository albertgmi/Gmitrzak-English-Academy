namespace inzBackend.Models.MatrixAssignmentModels
{
    public class MatrixAssignmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int MatrixId { get; set; }
        public string MatrixName { get; set; } = string.Empty;
        public int RefreshIntervalDays { get; set; }
        public DateOnly StartDate { get; set; }
        public List<ModuleUnlockDto> Modules { get; set; } = [];
    }
}
