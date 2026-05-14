using inzBackend.Models.StudentLearningModels.FlashcardModels;

namespace inzBackend.Services.StudentLearningServices.Flashcards
{
    public interface IFlashcardsService
    {
        List<FlashcardDto> getAllFlashcards();
        List<FlashcardDto> getLeeches();
        List<FlashcardDto> getStudiedToday();
        List<FlashcardStudyLogDto> getStudyLogs();
        List<FlashcardDto> searchFlashcards(string query);
    }
}