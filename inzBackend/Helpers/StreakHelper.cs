using System;
using System.Collections.Generic;
using System.Linq;

namespace inzBackend.Helpers
{
    public static class StreakHelper
    {
        public static int CalculateDynamicStreak(
            List<DateOnly> userStudyDates,
            int? streakOverride,
            DateOnly? streakOverrideDate,
            DateOnly today)
        {
            var sortedDates = userStudyDates.Distinct().OrderByDescending(x => x).ToList();

            if (!streakOverride.HasValue)
            {
                if (!sortedDates.Any() || sortedDates.First() < today.AddDays(-1))
                    return 0;

                var streak = 0;
                var expected = sortedDates.First();
                foreach (var d in sortedDates)
                {
                    if (d == expected)
                    {
                        streak++;
                        expected = expected.AddDays(-1);
                    }
                    else break;
                }
                return streak;
            }

            var overrideBaseDate = streakOverrideDate ?? today;
            var lastStudyDate = sortedDates.FirstOrDefault();
            var anchorDate = lastStudyDate > overrideBaseDate ? lastStudyDate : overrideBaseDate;

            // If anchor date is before yesterday, the streak has decayed to 0
            if (anchorDate < today.AddDays(-1))
            {
                return 0;
            }

            // If user hasn't studied after the override base date yet, but anchorDate is today/yesterday
            if (lastStudyDate <= overrideBaseDate)
            {
                return streakOverride.Value;
            }

            // Count consecutive study days strictly after overrideBaseDate
            var extraDays = 0;
            var currentCheck = lastStudyDate;
            foreach (var d in sortedDates)
            {
                if (d <= overrideBaseDate) break;
                if (d == currentCheck)
                {
                    extraDays++;
                    currentCheck = currentCheck.AddDays(-1);
                }
                else break;
            }

            if (currentCheck == overrideBaseDate)
            {
                return streakOverride.Value + extraDays;
            }
            else
            {
                var streakFromRecent = 0;
                var expected = lastStudyDate;
                foreach (var d in sortedDates)
                {
                    if (d == expected)
                    {
                        streakFromRecent++;
                        expected = expected.AddDays(-1);
                    }
                    else break;
                }
                return streakFromRecent;
            }
        }
    }
}
