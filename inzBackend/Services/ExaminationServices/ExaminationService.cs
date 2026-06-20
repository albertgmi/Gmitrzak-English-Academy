using inzBackend.Models.ExaminationModels;
using inzBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.ExaminationServices
{
    public class ExaminationService : IExaminationService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public ExaminationService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ExaminationDto GetExamination(int studentId)
        {
            var studiedFlashcardIds = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == studentId)
                .Select(x => x.FlashcardId)
                .Distinct()
                .ToList();

            var flashcards = _dbContext.Flashcards
                .Include(x => x.Vocabulary)
                .Where(x => x.UserId == studentId
                         && studiedFlashcardIds.Contains(x.Id)
                         && x.Vocabulary != null)
                .OrderBy(_ => Guid.NewGuid())
                .Take(20)
                .Select(x => new ExaminationFlashcardDto
                {
                    Id = x.Id,
                    Front = x.Vocabulary!.Front,
                    Back = x.Vocabulary!.Back,
                    Category = x.Vocabulary!.Category,
                    EaseFactor = x.EaseFactor,
                    Interval = x.Interval
                })
                .ToList();

            var sentences = _dbContext.Sentences
                .Where(x => x.UserId == studentId && x.IsReviewed)
                .OrderBy(_ => Guid.NewGuid())
                .Take(10)
                .Select(x => new ExaminationSentenceDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    Translation = x.Translation,
                    Notes = x.Notes
                })
                .ToList();

            var memories = _dbContext.Memories
                .Where(x => x.UserId == studentId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ExaminationMemoryDto
                {
                    Id = x.Id,
                    OptionA = x.OptionA,
                    OptionB = x.OptionB,
                    Content = x.Content,
                    Notes = x.Notes,
                    Category = x.Category
                })
                .ToList();

            return new ExaminationDto
            {
                Flashcards = flashcards,
                Sentences = sentences,
                Memories = memories
            };
        }
    }
}
