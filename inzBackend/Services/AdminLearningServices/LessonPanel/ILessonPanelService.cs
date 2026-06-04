using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AttendanceModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;

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
        StudentStudyTimeDto getStudyTime(int studentUserId);
        LessonLastWeekDto getLastWeek(int studentUserId);
        LessonStatsDto getStats(int studentUserId);
        List<AttendanceDto> getAttendance(int studentId);
        AttendanceDto addAttendance(CreateAttendanceDto dto);
        bool deleteAttendance(int id);
        List<AttendanceDto> getAttendanceHistory(int studentId);
        List<FlashcardDto> getAllFlashcardsForUser(int userId);
        void updateFlashcardInterval(int studentUserId, int flashcardId, int newInterval);
    }
}
