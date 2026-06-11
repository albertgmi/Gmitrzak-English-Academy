using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using AutoMapper;

namespace inzBackend.Services.StudentLearningServices.Memories
{
    public class MemoriesService : IMemoriesService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public MemoriesService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public List<MemoryDto> getAllMemories()
        {
            var userId = _userContextService.GetUserId;
            var memories = _dbContext
                .Memories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return _mapper.Map<List<MemoryDto>>(memories);
        }
    }
}
