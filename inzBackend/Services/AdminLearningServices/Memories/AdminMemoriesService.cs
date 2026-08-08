using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models;

using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Services.AdminLearningServices.Memories
{
    public class AdminMemoriesService : IAdminMemoriesService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AdminMemoriesService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AdminMemoryDto> GetStudentMemories(int studentId)
        {
            return _dbContext.Memories
                .Where(m => m.UserId == studentId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new AdminMemoryDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    Content = m.Content,
                    OptionA = m.OptionA,
                    OptionB = m.OptionB,
                    Notes = m.Notes,
                    Category = m.Category
                })
                .ToList();
        }

        public void UpdateMemory(int id, UpdateMemoryRequest request)
        {
            var memory = _dbContext.Memories.FirstOrDefault(m => m.Id == id);
            if (memory == null)
                throw new NotFoundException("Memory not found");

            memory.Content = request.Content;
            memory.OptionA = request.OptionA;
            memory.OptionB = request.OptionB;
            memory.Notes = request.Notes;
            memory.Category = request.Category;

            _dbContext.SaveChanges();
        }

        public void DeleteMemory(int id)
        {
            var memory = _dbContext.Memories.FirstOrDefault(m => m.Id == id);
            if (memory == null)
                throw new NotFoundException("Memory not found");

            _dbContext.Memories.Remove(memory);
            _dbContext.SaveChanges();
        }
    }
}
