using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Models.StudentLearningModels.SentenceModels;

namespace inzBackend.Services.StudentLearningServices.Sentences
{
    public interface ISentencesService
    {
        List<SentenceDto> getAllSentences();
        ModuleSentenceSessionDto getModuleSentences(int moduleId);
        void reviewSentence(int id, ReviewSentenceRequest request);
        List<SentenceDto> getOtherSentences();
    }
}