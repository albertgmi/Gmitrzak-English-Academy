namespace inzBackend.Models.AdminLearningModels
{
    public class LessonStudyLogDto
    {
        public DateOnly StudyDate { get; set; }
        public int EasyCount { get; set; }
        public int HardCount { get; set; }
        public int IncorrectCount { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}
