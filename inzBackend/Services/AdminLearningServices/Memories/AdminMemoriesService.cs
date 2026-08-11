using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Services.AdminLearningServices.Lesson;

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

        public int ImportMemoriesFromExcel(int studentId, Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("No file provided.");

            var studentExists = _dbContext.Users.Any(u => u.Id == studentId);
            if (!studentExists)
                throw new NotFoundException("Student not found.");

            var importedCount = 0;

            using var stream = file.OpenReadStream();
            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new BadRequestException("Excel worksheet is empty.");

            var rows = worksheet.RowsUsed().ToList();
            if (!rows.Any()) return 0;

            // Always skip row 1 (header row containing column names e.g. memory, notes)
            var startRowIndex = rows.Count > 1 ? 1 : 0;

            for (var i = startRowIndex; i < rows.Count; i++)
            {
                var row = rows[i];
                var optionA = row.Cell(1).GetString().Trim();
                var notes = row.Cell(2).GetString().Trim();

                if (string.IsNullOrWhiteSpace(optionA))
                    continue;

                _dbContext.Memories.Add(new Entities.SpacedRepetition.Memory
                {
                    UserId = studentId,
                    OptionA = optionA,
                    OptionB = null,
                    Content = optionA,
                    Notes = string.IsNullOrWhiteSpace(notes) ? null : notes,
                    Category = "blank"
                });

                importedCount++;
            }

            _dbContext.SaveChanges();
            return importedCount;
        }
    }
}
