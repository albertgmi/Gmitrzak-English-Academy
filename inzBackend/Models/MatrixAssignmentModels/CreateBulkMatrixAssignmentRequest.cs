namespace inzBackend.Models.MatrixAssignmentModels
{
    public class CreateBulkMatrixAssignmentRequest
    {
        public int MatrixId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public List<int> UserIds { get; set; } = new();
    }
}
