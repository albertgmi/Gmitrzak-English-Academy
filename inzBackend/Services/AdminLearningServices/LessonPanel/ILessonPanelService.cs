using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AttendanceModels;
using inzBackend.Models.CreditModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;

namespace inzBackend.Services.AdminLearningServices.LessonPanel
{
    public interface ILessonPanelService
    {
        AgendaDto GetAgenda(int studentUserId);
        void UpdateAgenda(int studentUserId, UpdateAgendaRequest request);
        List<LessonGradeDto> GetGrades(int studentUserId);
        ActivityPointsLessonSummaryDto GetActivityPoints(int studentUserId);
        void AddActivityPoints(int studentUserId, int points, string reason);
        LessonFlashcardSummaryDto GetFlashcardSummary(int studentUserId);
        StudentStudyTimeDto GetStudyTime(int studentUserId);
        LessonLastWeekDto GetLastWeek(int studentUserId);
        LessonStatsDto GetStats(int studentUserId);
        List<AttendanceDto> GetAttendance(int studentId);
        AttendanceDto AddAttendance(CreateAttendanceDto dto);
        bool DeleteAttendance(int id);
        List<AttendanceDto> GetAttendanceHistory(int studentId);
        List<FlashcardDto> GetAllFlashcardsForUser(int userId);
        void UpdateFlashcardInterval(int studentUserId, int flashcardId, int newInterval);
        ActivityScoreDto CalculateActivityScore(int studentUserId, DateOnly weekStart, DateOnly weekEnd);
    }
}
