using inzBackend.Models.ProgramModels;

namespace inzBackend.Services.ProgramServices
{
    public interface IProgramService
    {
        List<ProgramDto> getAllPrograms();
        void updateProgram(int programId, UpdateProgramRequest request);
        void deleteProgram(int programId);
        Entities.Curriculum.Program createProgram(CreateProgramRequest request);
    }
}
