using inzBackend.Models.StudentLearningModels.AssignmentStudentModels;

namespace inzBackend.Services.StudentLearningServices.Assignment
{
    public interface IStudentAssignmentService
    {
        List<AssignmentStudentDto> getActiveAssignments();
        List<AssignmentStudentDto> getAssignmentHistory();
    }
}