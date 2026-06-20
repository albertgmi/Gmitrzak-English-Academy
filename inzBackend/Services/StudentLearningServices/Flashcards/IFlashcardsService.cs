using inzBackend.Models.StudentLearningModels.FlashcardModels;

namespace inzBackend.Services.StudentLearningServices.Flashcards
{
    public interface IFlashcardsService
    {
        List<FlashcardDto> GetAllFlashcards();
        List<FlashcardDto> GetLeeches();
        List<FlashcardDto> GetStudiedToday();
        List<FlashcardStudyLogDto> GetStudyLogs();
        List<FlashcardDto> SearchFlashcards(string query);
        void ReviewCard(int flashcardId, ReviewCardRequest request);
    }
}