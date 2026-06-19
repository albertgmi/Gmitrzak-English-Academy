namespace inzBackend.Helpers
{
    public static class MatrixModuleDateHelper
    {
        public static DateOnly ComputeDeadline(DateOnly startDate, int weekNumber, int dayOfWeek, int refreshIntervalDays)
            => startDate.AddDays((weekNumber - 1) * refreshIntervalDays + (dayOfWeek - 1));

        public static DateOnly GetEffectiveDeadline(DateOnly startDate, int weekNumber, int dayOfWeek,
            int refreshIntervalDays, DateOnly? overrideDate)
            => overrideDate ?? ComputeDeadline(startDate, weekNumber, dayOfWeek, refreshIntervalDays);
    }
}
