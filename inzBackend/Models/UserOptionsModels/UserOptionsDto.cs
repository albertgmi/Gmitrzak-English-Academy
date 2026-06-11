namespace inzBackend.Models.UserOptionsModels
{
    public class UserOptionsDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int LeechThreshold { get; set; }
        public int MinDailyFlashcards { get; set; }
        public bool EmailNotifications { get; set; }
        public int IncorrectStepOneMinutes { get; set; }
        public int IncorrectStepTwoMinutes { get; set; }
        public int IncorrectStepThreeMinutes { get; set; }
        public int IncorrectStepFourMinutes { get; set; }
    }
}
