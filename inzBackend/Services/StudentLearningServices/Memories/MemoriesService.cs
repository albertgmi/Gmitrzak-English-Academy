using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Services.UserServices;

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

        public List<MemoryDto> GetAllMemories()
        {
            var userId = _userContextService.GetUserId;
            var memories = _dbContext
                .Memories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return _mapper.Map<List<MemoryDto>>(memories);
        }

        public void AddNotes(int memoryId, AddNotesRequest note)
        {
            var memory = _dbContext
                .Memories
                .Where(m => m.Id == memoryId)
                .FirstOrDefault();

            if (memory is null)
                throw new NotFoundException("Memory not found");

            memory.Notes = note.Notes;
            _dbContext.SaveChanges();
        }
    }
}
