using inzBackend.Models.ProgramModels;

namespace inzBackend.Services.ProgramServices
{
    public interface IProgramService
    {
        public List<ProgramDto> getAllPrograms();
        public void updateProgram(int programId, UpdateProgramRequest request);
        public void deleteProgram(int programId);
        public Models.Program createProgram(CreateProgramRequest request);
    }
}
