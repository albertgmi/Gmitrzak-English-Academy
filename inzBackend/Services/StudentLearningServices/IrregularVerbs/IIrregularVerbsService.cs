using inzBackend.Enums;
using inzBackend.Models.StudentLearningModels.IrregularVerbModels;

namespace inzBackend.Services.StudentLearningServices.IrregularVerbs
{
    public interface IIrregularVerbsService
    {
        List<IrregularVerbDto> GetIrregularVerbsByLevel(IrregularVerbLevel level);
        void ReviewIrregularVerb(int id, ReviewIrregularVerbRequest request);
    }
}
