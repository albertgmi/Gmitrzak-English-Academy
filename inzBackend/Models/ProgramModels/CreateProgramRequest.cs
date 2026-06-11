namespace inzBackend.Models.ProgramModels
{
    public class CreateProgramRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }
    }
}
