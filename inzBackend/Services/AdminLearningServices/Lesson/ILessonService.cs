using inzBackend.Entities;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.StudentLearningModels.AlphabetModels;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;

namespace inzBackend.Services.AdminLearningServices.Lesson
{
    public interface ILessonService
    {
        SearchGlobalFlashcardResult SearchGlobalFlashcard(string query, int studentUserId);
        VocabularyDto AddTranslation(AddTranslationRequest request);
        void AssignFlashcardToStudent(AssignFlashcardToStudentRequest request);
        void AddSentence(AddSentenceRequest request);
        void AddMemory(AddMemoryRequest request);
        void AddPronunciation(AddPronunciationRequest request);
        List<HomeworkItemDto> GetHomeworkForWeek(int studentUserId);
        void CheckHomework(int assignmentId);
        void UncheckHomework(int assignmentId);
        List<PronunciationTestItemDto> GetPronunciationList(int studentUserId);
        void CheckPronunciationWord(int entryId);
        void UncheckPronunciationWord(int entryId);
        void AddGrade(AddGradeRequest request);
        List<GradeListDto> GetGrades(int studentUserId);
        void RemoveGrade(int gradeId);
        List<TeacherNoteDto> GetNotes(int studentUserId);
        void SaveNote(SaveNoteRequest request);
        void DeleteNote(int noteId);
        void AddListeningReport(AddListeningReportRequest request);
        List<ListeningReportDto> GetListeningReports(int studentUserId);
        List<MemoryDto> GetMemories(int studentUserId);
        void MarkPronunciationResult(MarkPronunciationRequest request);
        List<PronunciationTestItemDto> GetCorrectEntries(int studentUserId);
        void AddAlphabetAbbreviation(AddAlphabetAbbreviationRequest request);
        List<AlphabetAbbreviationDto> GetAlphabetPool();
        void DeleteAlphabetAbbreviation(int id);
        List<AlphabetTestItemDto> GetAlphabetTestList(int studentUserId);
        List<AlphabetHistoryItemDto> GetAlphabetHistory(int studentUserId);
        void MarkAlphabetResult(MarkAlphabetRequest request);
        List<AlphabetAttemptDto> GetAlphabetEntryAttempts(int entryId);
    }
}
