namespace inzBackend.Helpers
{
    public static class PolandTime
    {
        private static readonly TimeZoneInfo PolishZone =
            TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        public static DateTimeOffset Now => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, PolishZone);
        public static DateTime DateTimeNow => TimeZoneInfo.ConvertTime(DateTime.UtcNow, PolishZone);
        public static DateOnly Today => DateOnly.FromDateTime(DateTimeNow);
        public static DateTimeOffset Convert(DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, PolishZone);
        }
    }
}
