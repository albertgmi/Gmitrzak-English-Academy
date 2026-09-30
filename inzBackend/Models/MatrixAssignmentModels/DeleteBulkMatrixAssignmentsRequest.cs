using System.Collections.Generic;

namespace inzBackend.Models.MatrixAssignmentModels
{
    public class DeleteBulkMatrixAssignmentsRequest
    {
        public List<int> AssignmentIds { get; set; } = new();
    }
}
