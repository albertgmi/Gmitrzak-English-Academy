using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using AutoMapper;

namespace inzBackend.Services.StudentLearningServices.Pronunciation
{
    public class PronunciationService : IPronunciationService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public PronunciationService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public List<PronunciationEntryDto> getAllEntries()
        {
            var userId = _userContextService.GetUserId;
            var entries = _dbContext.PronunciationEntries
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.IsChecked)
                .ThenBy(x => x.SortOrder)
                .ToList();

            return _mapper.Map<List<PronunciationEntryDto>>(entries);
        }

        public void checkEntry(int id)
        {
            var userId = _userContextService.GetUserId;
            var entry = _dbContext.PronunciationEntries
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);
            if (entry is null) return;
            entry.IsChecked = true;
            _dbContext.SaveChanges();
        }

        public void uncheckEntry(int id)
        {
            var userId = _userContextService.GetUserId;
            var entry = _dbContext.PronunciationEntries
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);
            if (entry is null) return;
            entry.IsChecked = false;
            _dbContext.SaveChanges();
        }
    }
}
