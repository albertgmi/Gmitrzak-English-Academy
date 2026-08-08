using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Services.AdminLearningServices.Memories
{
    public interface IAdminMemoriesService
    {
        List<AdminMemoryDto> GetStudentMemories(int studentId);
        void UpdateMemory(int id, UpdateMemoryRequest request);
        void DeleteMemory(int id);
    }
}
