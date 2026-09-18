using System;
using System.Collections.Generic;
using System.Linq;

namespace inzBackend.Helpers
{
    public static class StreakHelper
    {
        public static int CalculateDynamicStreak(
            IEnumerable<DateOnly> userStudyDates,
            int? streakOverride,
            DateOnly? streakOverrideDate,
            DateOnly today,
            IEnumerable<DateOnly>? shieldedDates = null)
        {
            var activeSet = userStudyDates.Distinct().ToHashSet();
            if (shieldedDates != null)
            {
                foreach (var s in shieldedDates)
                {
                    activeSet.Add(s);
                }
            }

            var sortedDates = activeSet.OrderByDescending(x => x).ToList();

            // Calculate natural consecutive streak
            var naturalStreak = CalculateNaturalStreak(activeSet, today);

            if (!streakOverride.HasValue)
            {
                return naturalStreak;
            }

            var overrideBaseDate = streakOverrideDate ?? sortedDates.FirstOrDefault();
            if (overrideBaseDate == default)
            {
                return naturalStreak;
            }

            // Check if streak is currently active (student was active/shielded today or yesterday)
            DateOnly? checkDate = activeSet.Contains(today)
                ? today
                : (activeSet.Contains(today.AddDays(-1)) ? today.AddDays(-1) : (DateOnly?)null);

            // If neither today nor yesterday is active/shielded, the streak is broken (0)
            if (!checkDate.HasValue)
            {
                return 0;
            }

            // If checkDate is before overrideBaseDate (e.g. override was set today, student hasn't studied today yet but studied yesterday):
            // The streak is active and override is valid.
            if (checkDate.Value < overrideBaseDate)
            {
                return streakOverride.Value;
            }

            // If checkDate >= overrideBaseDate, verify that every day after overrideBaseDate up to checkDate was active
            var curr = checkDate.Value;
            var extraDays = 0;
            var isContinuation = true;

            while (curr > overrideBaseDate)
            {
                if (!activeSet.Contains(curr))
                {
                    isContinuation = false;
                    break;
                }
                extraDays++;
                curr = curr.AddDays(-1);
            }

            if (isContinuation)
            {
                return streakOverride.Value + extraDays;
            }

            // If chain was broken between overrideBaseDate and checkDate, fall back to natural consecutive streak
            return naturalStreak;
        }

        private static int CalculateNaturalStreak(HashSet<DateOnly> activeSet, DateOnly today)
        {
            if (!activeSet.Any()) return 0;

            DateOnly? checkDate = activeSet.Contains(today)
                ? today
                : (activeSet.Contains(today.AddDays(-1)) ? today.AddDays(-1) : (DateOnly?)null);

            if (!checkDate.HasValue) return 0;

            var streak = 0;
            var current = checkDate.Value;

            while (activeSet.Contains(current))
            {
                streak++;
                current = current.AddDays(-1);
            }

            return streak;
        }
    }
}

