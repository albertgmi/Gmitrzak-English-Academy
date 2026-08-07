using inzBackend.Entities.LearningMaterials;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.AlphabetModels;
using inzBackend.Services.UserServices;

namespace inzBackend.Services.StudentLearningServices.Alphabet
{
    public class AlphabetService : IAlphabetService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        private const int LETTER_SEQUENCES_PER_WEEK = 15;
        private const int LETTERS_PER_SEQUENCE = 7;
        private const int ABBREVIATIONS_PER_WEEK = 10;
        private const int NO_REPEAT_WEEKS = 2;

        private static readonly char[] Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly Random Rng = new();

        public AlphabetService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public List<AlphabetEntryDto> GetCurrentWeekEntries()
        {
            int userId = _userContextService.GetUserId!.Value;

            var latestWeek = _dbContext.AlphabetEntries
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.WeekStartDate)
                .Select(x => (DateOnly?)x.WeekStartDate)
                .FirstOrDefault();

            if (latestWeek is null) return new List<AlphabetEntryDto>();

            return _dbContext.AlphabetEntries
                .Where(x => x.UserId == userId && x.WeekStartDate == latestWeek)
                .OrderBy(x => x.Type)
                .ThenBy(x => x.SortOrder)
                .Select(x => new AlphabetEntryDto
                {
                    Id = x.Id,
                    Type = x.Type.ToString(),
                    Content = x.Content,
                    Status = x.Status.ToString(),
                    SortOrder = x.SortOrder,
                    WeekStartDate = x.WeekStartDate
                })
                .ToList();
        }

        public List<AlphabetAttemptDto> GetAttempts(int entryId)
        {
            int userId = _userContextService.GetUserId!.Value;

            var exists = _dbContext.AlphabetEntries.Any(x => x.Id == entryId && x.UserId == userId);
            if (!exists) throw new NotFoundException("Alphabet entry not found");

            return _dbContext.AlphabetAttempts
                .Where(x => x.AlphabetEntryId == entryId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AlphabetAttemptDto
                {
                    Id = x.Id,
                    ProblemLetters = x.ProblemLetters,
                    Feedback = x.Feedback,
                    CreatedAt = PolandTime.Convert(x.CreatedAt).DateTime,
                })
                .ToList();
        }

        public void GenerateWeeklyProgram()
        {
            int userId = _userContextService.GetUserId!.Value;
            var weekStart = WeekHelper.GetWeekMonday(PolandTime.Today);

            var alreadyGenerated = _dbContext.AlphabetEntries
                .Any(x => x.UserId == userId && x.WeekStartDate == weekStart);

            if (alreadyGenerated)
                throw new BadRequestException("This week's alphabet program was already generated.");

            var recentCutoff = weekStart.AddDays(-7 * NO_REPEAT_WEEKS);

            var recentSequences = _dbContext.AlphabetEntries
                .Where(x => x.UserId == userId
                         && x.Type == AlphabetEntryType.Letters
                         && x.WeekStartDate >= recentCutoff)
                .Select(x => x.Content)
                .ToHashSet();

            var newEntries = new List<AlphabetEntry>();

            for (int i = 0; i < LETTER_SEQUENCES_PER_WEEK; i++)
            {
                string sequence;
                var attempts = 0;
                do
                {
                    sequence = GenerateLetterSequence();
                    attempts++;
                } while (recentSequences.Contains(sequence) && attempts < 50);

                recentSequences.Add(sequence);

                newEntries.Add(new AlphabetEntry
                {
                    UserId = userId,
                    WeekStartDate = weekStart,
                    Type = AlphabetEntryType.Letters,
                    Content = sequence,
                    Status = PronunciationStatus.Pending,
                    SortOrder = i + 1
                });
            }

            var recentAbbreviationIds = _dbContext.AlphabetEntries
                .Where(x => x.UserId == userId
                         && x.Type == AlphabetEntryType.Abbreviation
                         && x.WeekStartDate >= recentCutoff)
                .Select(x => x.AbbreviationId!.Value)
                .ToHashSet();

            var pool = _dbContext.AlphabetAbbreviations.ToList();
            if (pool.Count == 0)
                throw new BadRequestException("The abbreviation pool is empty. Ask your teacher to add some.");

            var freshPool = pool.Where(x => !recentAbbreviationIds.Contains(x.Id)).ToList();
            var pickFrom = freshPool.Count >= ABBREVIATIONS_PER_WEEK ? freshPool : pool;

            var chosen = pickFrom.OrderBy(_ => Rng.Next()).Take(ABBREVIATIONS_PER_WEEK).ToList();

            for (int i = 0; i < chosen.Count; i++)
            {
                newEntries.Add(new AlphabetEntry
                {
                    UserId = userId,
                    WeekStartDate = weekStart,
                    Type = AlphabetEntryType.Abbreviation,
                    Content = chosen[i].Text,
                    AbbreviationId = chosen[i].Id,
                    Status = PronunciationStatus.Pending,
                    SortOrder = i + 1
                });
            }

            _dbContext.AlphabetEntries.AddRange(newEntries);
            _dbContext.SaveChanges();
        }

        private static string GenerateLetterSequence()
        {
            var result = new char[LETTERS_PER_SEQUENCE];
            char? previous = null;

            for (int i = 0; i < LETTERS_PER_SEQUENCE; i++)
            {
                char next;
                do
                {
                    next = Alphabet[Rng.Next(Alphabet.Length)];
                } while (next == previous);

                result[i] = next;
                previous = next;
            }

            return new string(result);
        }
    }
}
