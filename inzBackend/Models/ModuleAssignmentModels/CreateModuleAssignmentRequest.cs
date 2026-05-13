namespace inzBackend.Models.ModuleAssignmentModels
{
    public class CreateModuleAssignmentRequest
    {
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public string DueDate { get; set; } = string.Empty;
    }
}
