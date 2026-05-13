namespace inzBackend.Models.AssignmentModels
{
    public class CreateAssignmentRequest
    {
        public int UserId { get; set; }
        public int MatrixId { get; set; }
        public DateOnly StartDate { get; set; }
    }
}
