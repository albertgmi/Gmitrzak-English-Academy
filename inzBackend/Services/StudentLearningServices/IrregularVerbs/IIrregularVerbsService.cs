using inzBackend.Enums;
using inzBackend.Models.StudentLearningModels.IrregularVerbModels;

namespace inzBackend.Services.StudentLearningServices.IrregularVerbs
{
    public interface IIrregularVerbsService
    {
        List<IrregularVerbDto> GetIrregularVerbsByLevel(IrregularVerbLevel level);
        List<IrregularVerbDto> GetAllIrregularVerbs();
        List<IrregularVerbDto> GetLeeches();
        List<IrregularVerbDto> GetStudiedToday();
        List<IrregularVerbDto> SearchIrregularVerbs(string query);
        void ReviewIrregularVerb(int id, ReviewIrregularVerbRequest request);
    }
}
