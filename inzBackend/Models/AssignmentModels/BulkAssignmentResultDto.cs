namespace inzBackend.Models.AssignmentModels
{
    public class BulkAssignmentResultDto
    {
        public List<string> AssignedUsernames { get; set; } = new();
        public List<string> Skipped { get; set; } = new();
    }
}
