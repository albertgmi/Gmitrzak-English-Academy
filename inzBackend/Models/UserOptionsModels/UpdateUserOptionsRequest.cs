namespace inzBackend.Models.UserOptionsModels
{
    public class UpdateUserOptionsRequest
    {
        public int LeechThreshold { get; set; }
        public int MinDailyFlashcards { get; set; }
        public bool EmailNotifications { get; set; }
        public int IncorrectStepOneMinutes { get; set; }
        public int IncorrectStepTwoMinutes { get; set; }
        public int IncorrectStepThreeMinutes { get; set; }
        public int IncorrectStepFourMinutes { get; set; }
    }
}
