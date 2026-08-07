using inzBackend.Models.StudentLearningModels.AlphabetModels;

namespace inzBackend.Services.StudentLearningServices.Alphabet
{
    public interface IAlphabetService
    {
        List<AlphabetEntryDto> GetCurrentWeekEntries();
        List<AlphabetAttemptDto> GetAttempts(int entryId);
        void GenerateWeeklyProgram();
    }
}
