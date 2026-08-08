using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiUsageGuardService : IAiUsageGuardService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private const int MAX_ATTEMPTS_PER_DAY = 200;
        private const int MIN_SECONDS_BETWEEN_ATTEMPTS = 3;

        public AiUsageGuardService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void EnsureCanSubmitAttempt(int userId)
        {
            var todayStart = PolandTime.Today.ToDateTime(TimeOnly.MinValue);
            var now = PolandTime.DateTimeNow;

            var pronunciationCountToday = _dbContext.PronunciationAttempts
                .Count(x => x.UserId == userId && x.CreatedAt >= todayStart);

            var alphabetCountToday = _dbContext.AlphabetAttempts
                .Count(x => x.UserId == userId && x.CreatedAt >= todayStart);

            var totalToday = pronunciationCountToday + alphabetCountToday;

            if (totalToday >= MAX_ATTEMPTS_PER_DAY)
            {
                throw new TooManyAttemptsException(
                    $"You've reached today's limit of {MAX_ATTEMPTS_PER_DAY} pronunciation checks. Please try again tomorrow.");
            }

            var lastPronunciation = _dbContext.PronunciationAttempts
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => (DateTimeOffset?)x.CreatedAt)
                .FirstOrDefault();

            var lastAlphabet = _dbContext.AlphabetAttempts
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => (DateTimeOffset?)x.CreatedAt)
                .FirstOrDefault();

            var lastAttempt = new[] { lastPronunciation, lastAlphabet }
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .DefaultIfEmpty()
                .Max();

            if (lastAttempt != default && (now - lastAttempt).TotalSeconds < MIN_SECONDS_BETWEEN_ATTEMPTS)
            {
                throw new TooManyAttemptsException(
                    "Please wait a few seconds before submitting another recording.");
            }
        }
    }
}