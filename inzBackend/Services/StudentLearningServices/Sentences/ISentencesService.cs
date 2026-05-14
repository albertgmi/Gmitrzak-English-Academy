using inzBackend.Models.StudentLearningModels.SentenceModels;

namespace inzBackend.Services.StudentLearningServices.Sentences
{
    public interface ISentencesService
    {
        List<SentenceDto> getAllSentences();
    }
}