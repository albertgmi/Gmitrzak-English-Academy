namespace inzBackend.Models.ProgramModels
{
    public class UpdateProgramRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? isHidden { get; set; }
    }
}
