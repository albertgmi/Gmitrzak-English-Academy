using System.Collections.Generic;

namespace inzBackend.Models.ModuleAssignmentModels
{
    public class CreateBulkModuleAssignmentRequest
    {
        public int ModuleId { get; set; }
        public string DueDate { get; set; } = string.Empty;
        public List<int> UserIds { get; set; } = new();
    }
}
