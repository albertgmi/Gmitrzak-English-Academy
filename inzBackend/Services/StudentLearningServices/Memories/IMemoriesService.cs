using inzBackend.Models.StudentLearningModels.MemoryModels;

namespace inzBackend.Services.StudentLearningServices.Memories
{
    public interface IMemoriesService
    {
        List<MemoryDto> GetAllMemories();
    }
}