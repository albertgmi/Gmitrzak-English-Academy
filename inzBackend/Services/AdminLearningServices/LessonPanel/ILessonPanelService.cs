using inzBackend.Models.AdminLearningModels;

namespace inzBackend.Services.AdminLearningServices.LessonPanel
{
    public interface ILessonPanelService
    {
        AgendaDto getAgenda(int studentUserId);
        void updateAgenda(int studentUserId, UpdateAgendaRequest request);
        List<LessonGradeDto> getGrades(int studentUserId);
        ActivityPointsLessonSummaryDto getActivityPoints(int studentUserId);
        void addActivityPoints(int studentUserId, int points, string reason);
        LessonFlashcardSummaryDto getFlashcardSummary(int studentUserId);
        List<StreamEntryDto> getStreamEntries(int studentUserId);
        void addStreamEntry(int studentUserId, string command, string payload);
        void deleteStreamEntry(int entryId);
        StudentStudyTimeDto getStudyTime(int studentUserId);
        LessonLastWeekDto getLastWeek(int studentUserId);
        LessonStatsDto getStats(int studentUserId);
    }
}
