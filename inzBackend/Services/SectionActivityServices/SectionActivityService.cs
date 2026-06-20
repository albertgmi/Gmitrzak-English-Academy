using inzBackend.Entities;
using inzBackend.Entities.Assignments;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.CreditServices;
using inzBackend.Services.UserServices;

namespace inzBackend.Services.SectionActivityServices
{
    public class SectionActivityService : ISectionActivityService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly ILessonPanelService _lessonPanelService;
        private readonly ICreditService _creditService;

        public SectionActivityService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            ILessonPanelService lessonPanelService, ICreditService creditService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _lessonPanelService = lessonPanelService;
            _creditService = creditService;
        }

        public void LogActivity(LogActivityRequest request)
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

                var points = request.Section.ToLower() switch
                {
                    "memories" => 3,
                    "pronunciation" => 3,
                    "sentenceflashcards" => 2,
                    "flashcards" => 2,
                    _ => 1
                };

                _lessonPanelService.AddActivityPoints(
                    userId, points, $"Daily visit: {request.Section.ToLower()}");

                _dbContext.SaveChanges();

                if (request.Section.ToLower() == "flashcards" ||
                    request.Section.ToLower() == "sentenceflashcards")
                {
                    _creditService.CheckAndAwardDailyChallenge(userId);
                    _creditService.CheckAndAwardWeeklyChallenge(userId);
                }
            }
        }
    }
}
