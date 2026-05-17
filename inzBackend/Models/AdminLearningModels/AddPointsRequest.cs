namespace inzBackend.Models.AdminLearningModels
{
    public class AddPointsRequest
    {
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
