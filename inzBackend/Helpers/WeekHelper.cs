namespace inzBackend.Helpers
{
    public static class WeekHelper
    {
        public static DateOnly GetWeekMonday(DateOnly date)
        {
            var daysFromMonday = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-daysFromMonday);
        }
    }
}
