using inzBackend.Models.ProgramModels;

namespace inzBackend.Services.ProgramServices
{
    public interface IProgramService
    {
        List<ProgramDto> GetAllPrograms();
        void UpdateProgram(int programId, UpdateProgramRequest request);
        void DeleteProgram(int programId);
        Entities.Curriculum.Program CreateProgram(CreateProgramRequest request);
    }
}
