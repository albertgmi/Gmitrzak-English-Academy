using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Models.StudentLearningModels.SentenceModels;

namespace inzBackend.Services.StudentLearningServices.Sentences
{
    public interface ISentencesService
    {
        List<SentenceDto> GetAllSentences();
        ModuleSentenceSessionDto GetModuleSentences(int moduleId);
        void ReviewSentence(int id, ReviewSentenceRequest request);
    }
}