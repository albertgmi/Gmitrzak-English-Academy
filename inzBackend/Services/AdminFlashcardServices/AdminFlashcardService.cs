using inzBackend.Models;
using inzBackend.Models.AdminFlashcardModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.AdminFlashcardServices
{
    public class AdminFlashcardService : IAdminFlashcardService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AdminFlashcardService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<AdminStudentStudySummaryDto> getAdminStudyLogsSummary()
        {
            return _dbContext.Users
                .Select(user => new AdminStudentStudySummaryDto
                {
                    Username = user.Username,

                    TotalTimeSpentSeconds = user.FlashcardStudyLogs.Any()
                        ? user.FlashcardStudyLogs.Sum(log => log.TimeSpentSeconds)
                        : null,

                    TotalFlashcardsDone = user.FlashcardStudyLogs.Any()
                        ? user.FlashcardStudyLogs.Sum(log =>
                            log.EasyCount +
                            log.HardCount +
                            log.IncorrectCount)
                        : null
                })
                .ToList();
        }
    }
}
