using inzBackend.Models.AdminLearningModels;
using Microsoft.AspNetCore.Http;

namespace inzBackend.Services.AdminLearningServices.Memories
{
    public interface IAdminMemoriesService
    {
        List<AdminMemoryDto> GetStudentMemories(int studentId);
        void UpdateMemory(int id, UpdateMemoryRequest request);
        void DeleteMemory(int id);
        int ImportMemoriesFromExcel(int studentId, IFormFile file);
    }
}
