using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AttendanceModels;

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
        IEnumerable<AttendanceDto> getAttendance(int studentId);
        AttendanceDto addAttendance(CreateAttendanceDto dto);
        bool deleteAttendance(int id);
        IEnumerable<AttendanceDto> getAttendanceHistory(int studentId);
    }
}
