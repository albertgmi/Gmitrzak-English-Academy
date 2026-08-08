using inzBackend.Entities;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Services.AdminLearningServices.Pronunciation
{
    public class AdminPronunciationService : IAdminPronunciationService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AdminPronunciationService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AdminPronunciationDto> GetStudentPronunciation(int studentId)
        {
            return _dbContext.PronunciationEntries
                .Where(p => p.UserId == studentId)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .Select(p => new AdminPronunciationDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Word = p.Word,
                    Status = p.Status.ToString(),
                    SortOrder = p.SortOrder,
                    IsInCurrentSession = p.IsInCurrentSession,
                    MarkedCorrectAt = p.MarkedCorrectAt
                })
                .ToList();
        }

        public void UpdatePronunciation(int id, UpdatePronunciationRequest request)
        {
            var entry = _dbContext.PronunciationEntries.FirstOrDefault(p => p.Id == id);
            if (entry == null)
                throw new NotFoundException("Pronunciation entry not found");

            if (Enum.TryParse<PronunciationStatus>(request.Status, true, out var parsedStatus))
            {
                entry.Status = parsedStatus;
            }

            entry.Word = request.Word;
            entry.SortOrder = request.SortOrder;
            entry.IsInCurrentSession = request.IsInCurrentSession;

            _dbContext.SaveChanges();
        }

        public void DeletePronunciation(int id)
        {
            var entry = _dbContext.PronunciationEntries.FirstOrDefault(p => p.Id == id);
            if (entry == null)
                throw new NotFoundException("Pronunciation entry not found");

            _dbContext.PronunciationEntries.Remove(entry);
            _dbContext.SaveChanges();
        }
    }
}
