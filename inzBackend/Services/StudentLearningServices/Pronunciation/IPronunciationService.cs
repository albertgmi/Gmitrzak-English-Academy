using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;

namespace inzBackend.Services.StudentLearningServices.Pronunciation
{
    public interface IPronunciationService
    {
        List<PronunciationEntryDto> GetAllEntries();
        List<PronunciationTestItemDto> GetCorrectPronunciation();
        List<PronunciationAttemptDto> GetAttempts(int pronunciationEntryId);
    }
}