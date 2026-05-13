namespace inzBackend.Models.MatrixAssignmentModels
{
    public class CreateMatrixAssignmentRequest
    {
        public int UserId { get; set; }
        public int MatrixId { get; set; }
        public string StartDate { get; set; } = string.Empty;
    }
}
