using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Services.SectionActivityServices
{
    public class SectionActivityService : ISectionActivityService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public SectionActivityService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public void logActivity(LogActivityRequest request)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var validSections = new[] { "memories", "pronunciation", "sentenceflashcards", "flashcards" };
            if (!validSections.Contains(request.Section.ToLower()))
                throw new BadRequestException("No valid sections");

            var exists = _dbContext.SectionActivityLogs.Any(x =>
                x.UserId == userId &&
                x.Section == request.Section.ToLower() &&
                x.ActivityDate == today);

            if (!exists)
            {
                _dbContext.SectionActivityLogs.Add(new SectionActivityLog
                {
                    UserId = userId,
                    Section = request.Section.ToLower(),
                    ActivityDate = today
                });
                _dbContext.SaveChanges();
            }
        }
    }
}
