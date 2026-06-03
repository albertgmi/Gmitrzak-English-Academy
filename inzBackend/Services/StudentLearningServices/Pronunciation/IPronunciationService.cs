using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;

namespace inzBackend.Services.StudentLearningServices.Pronunciation
{
    public interface IPronunciationService
    {
        List<PronunciationEntryDto> getAllEntries();
        List<PronunciationTestItemDto> getCorrectPronunciation();
    }
}