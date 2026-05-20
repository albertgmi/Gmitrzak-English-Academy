using inzBackend.Models;

namespace inzBackend.Entities
{
    public class UserOptions : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int LeechThreshold { get; set; } = 150;
        public int MinDailyFlashcards { get; set; } = 20;
        public bool EmailNotifications { get; set; } = true;
        public int IncorrectStepOneMinutes { get; set; } = 3;
        public int IncorrectStepTwoMinutes { get; set; } = 6;
        public int IncorrectStepThreeMinutes { get; set; } = 10;
        public int IncorrectStepFourMinutes { get; set; } = 15;
    }
}
