using AutoMapper;
using inzBackend.Entities.LearningMaterials;
using inzBackend.Enums;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.IrregularVerbModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.CreditServices;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentLearningServices.IrregularVerbs
{
    public class IrregularVerbsService : IIrregularVerbsService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly ILessonPanelService _lessonPanelService;
        private readonly ICreditService _creditService;

        public IrregularVerbsService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService,
            IMapper mapper,
            ILessonPanelService lessonPanelService,
            ICreditService creditService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
            _lessonPanelService = lessonPanelService;
            _creditService = creditService;
        }

        public List<IrregularVerbDto> GetIrregularVerbsByLevel(IrregularVerbLevel level)
        {
            var userId = _userContextService.GetUserId;
            if (userId is null) return new List<IrregularVerbDto>();

            var verbs = _dbContext.IrregularVerbs
                .Where(x => x.UserId == userId.Value && x.Level == level)
                .OrderBy(x => x.NextReviewDate)
                .ToList();

            return _mapper.Map<List<IrregularVerbDto>>(verbs);
        }

        public void ReviewIrregularVerb(int id, ReviewIrregularVerbRequest request)
        {
            var userId = _userContextService.GetUserId;
            if (userId is null) return;

            var card = _dbContext.IrregularVerbs
                .FirstOrDefault(x => x.Id == id && x.UserId == userId.Value);

            if (card is null) return;

            var today = PolandTime.Today;

            switch (request.Quality.ToLower())
            {
                case "easy":
                    card.Interval = card.Interval == 0 ? 2 : card.Interval * 2;
                    card.NextReviewDate = today.AddDays(card.Interval);
                    card.EaseFactor = Math.Min(card.EaseFactor + 10, 300);
                    break;

                case "hard":
                    card.Interval = 1;
                    card.NextReviewDate = today.AddDays(1);
                    card.EaseFactor = Math.Max(card.EaseFactor - 15, 130);
                    break;

                case "incorrect":
                case "again_1m":
                    card.Interval = 0;
                    card.NextReviewDate = today;
                    card.EaseFactor = Math.Max(card.EaseFactor - 20, 130);
                    break;
            }

            card.IsLeech = card.EaseFactor <= 150;

            _lessonPanelService.AddActivityPoints(userId.Value, 2, "Irregular verb flashcard done");
            _creditService.CheckAndAwardDailyChallenge(userId.Value);
            _creditService.CheckAndAwardWeeklyChallenge(userId.Value);

            _dbContext.SaveChanges();
        }
    }
}
