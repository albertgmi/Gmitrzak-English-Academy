using AutoMapper;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.SentenceModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentLearningServices.Sentences
{
    public class SentencesService : ISentencesService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public SentencesService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public List<SentenceDto> getAllSentences()
        {
            var userId = _userContextService.GetUserId;
            var sentences = _dbContext
                .Sentences
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return _mapper.Map<List<SentenceDto>>(sentences);
        }
    }
}
