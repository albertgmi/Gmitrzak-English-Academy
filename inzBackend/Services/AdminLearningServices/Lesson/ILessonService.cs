using inzBackend.Entities;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;

namespace inzBackend.Services.AdminLearningServices.Lesson
{
    public interface ILessonService
    {
        SearchGlobalFlashcardResult searchGlobalFlashcard(string query, int studentUserId);
        VocabularyDto addTranslation(AddTranslationRequest request);
        void assignFlashcardToStudent(AssignFlashcardToStudentRequest request);
        void addSentence(AddSentenceRequest request);
        void addMemory(AddMemoryRequest request);
        void addPronunciation(AddPronunciationRequest request);
        List<HomeworkItemDto> getHomeworkForWeek(int studentUserId);
        void checkHomework(int assignmentId);
        void uncheckHomework(int assignmentId);
        List<PronunciationTestItemDto> getPronunciationList(int studentUserId);
        void checkPronunciationWord(int entryId);
        void uncheckPronunciationWord(int entryId);
        void addGrade(AddGradeRequest request);
        List<GradeListDto> getGrades(int studentUserId);
        void removeGrade(int gradeId);
        List<TeacherNoteDto> getNotes(int studentUserId);
        void saveNote(SaveNoteRequest request);
        void deleteNote(int noteId);
        void addListeningReport(AddListeningReportRequest request);
        List<ListeningReportDto> getListeningReports(int studentUserId);
    }
}
