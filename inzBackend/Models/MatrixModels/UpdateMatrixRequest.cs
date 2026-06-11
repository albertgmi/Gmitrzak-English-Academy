namespace inzBackend.Models.MatrixModels
{
    public class UpdateMatrixRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RefreshIntervalDays { get; set; }
        public bool? IsHidden { get; set; }
    }
}
