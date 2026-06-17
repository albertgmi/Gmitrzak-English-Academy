using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;

namespace inzBackend.Services.StudentLearningServices.Pronunciation
{
    public interface IPronunciationService
    {
        List<PronunciationEntryDto> getAllEntries();
        List<PronunciationTestItemDto> getCorrectPronunciation();
        List<PronunciationAttemptDto> getAttempts(int pronunciationEntryId);
    }
}