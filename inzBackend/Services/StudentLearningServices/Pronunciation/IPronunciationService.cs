using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;

namespace inzBackend.Services.StudentLearningServices.Pronunciation
{
    public interface IPronunciationService
    {
        List<PronunciationEntryDto> getAllEntries();
        void checkEntry(int id);
        void uncheckEntry(int id);
    }
}