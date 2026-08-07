using inzBackend.Entities.LearningMaterials;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.AlphabetModels;
using inzBackend.Services.UserServices;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiAlphabetService : IAiAlphabetService
    {
        private readonly IUserContextService _userContextService;
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly string _azureSubscriptionKey;
        private readonly string _azureRegion;

        private const int LETTER_PASS_THRESHOLD = 50;

        private static readonly Dictionary<char, string> LetterToPhonetic = new()
        {
            {'A', "ay"}, {'B', "bee"}, {'C', "cee"}, {'D', "dee"}, {'E', "ee"},
            {'F', "eff"}, {'G', "gee"}, {'H', "aitch"}, {'I', "eye"}, {'J', "jay"},
            {'K', "kay"}, {'L', "el"}, {'M', "em"}, {'N', "en"}, {'O', "oh"},
            {'P', "pee"}, {'Q', "cue"}, {'R', "ar"}, {'S', "ess"}, {'T', "tee"},
            {'U', "you"}, {'V', "vee"}, {'W', "doubleyou"}, {'X', "ex"}, {'Y', "why"}, {'Z', "zee"},
            {'0', "zero"}, {'1', "one"}, {'2', "two"}, {'3', "three"}, {'4', "four"},
            {'5', "five"}, {'6', "six"}, {'7', "seven"}, {'8', "eight"}, {'9', "nine"}
        };

        private static readonly Dictionary<string, string> PhoneticToLetter = LetterToPhonetic
            .ToDictionary(kvp => kvp.Value, kvp => kvp.Key.ToString(), StringComparer.OrdinalIgnoreCase);

        public AiAlphabetService(
            IUserContextService userContextService,
            GmitrzakEnglishAcademyDbContext dbContext,
            IConfiguration configuration)
        {
            _userContextService = userContextService;
            _dbContext = dbContext;
            _azureSubscriptionKey = configuration["AzureSpeechSettings:SubscriptionKey"]
                ?? throw new InvalidOperationException("AzureSpeechSettings:SubscriptionKey is missing in configuration.");
            _azureRegion = configuration["AzureSpeechSettings:Region"]
                ?? throw new InvalidOperationException("AzureSpeechSettings:Region is missing in configuration.");
        }

        public async Task<AlphabetResult> ProcessUserAttemptAsync(Stream audioStream, string fileName, int alphabetEntryId)
        {
            int userId = _userContextService.GetUserId!.Value;

            var entry = await _dbContext.AlphabetEntries
                .FirstOrDefaultAsync(x => x.Id == alphabetEntryId && x.UserId == userId);

            if (entry == null)
                throw new NotFoundException("Alphabet entry not found");

            var rawLetters = entry.Content
                .Where(char.IsLetterOrDigit)
                .Select(c => char.ToUpper(c))
                .ToList();

            if (!rawLetters.Any())
                throw new InvalidOperationException("Alphabet entry contains no valid letters or digits.");

            var phoneticWords = rawLetters
                .Select(c => LetterToPhonetic.TryGetValue(c, out var phonetic) ? phonetic : c.ToString().ToLower())
                .ToList();

            var referenceText = string.Join(", ", phoneticWords) + ".";

            var speechConfig = SpeechConfig.FromSubscription(_azureSubscriptionKey, _azureRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";
            speechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, "3000");

            using var memoryStream = new MemoryStream();
            await audioStream.CopyToAsync(memoryStream);
            byte[] audioBytes = memoryStream.ToArray();

            using var pushStream = AudioInputStream.CreatePushStream();
            pushStream.Write(audioBytes);
            pushStream.Close();

            using var audioConfig = AudioConfig.FromStreamInput(pushStream);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var pronConfig = new PronunciationAssessmentConfig(
                referenceText: referenceText,
                gradingSystem: GradingSystem.HundredMark,
                granularity: Granularity.Word,
                enableMiscue: true
            );
            pronConfig.ApplyTo(recognizer);

            var result = await recognizer.RecognizeOnceAsync();

            var problemLetters = new List<string>();
            string feedbackMessage;

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                var pronResult = PronunciationAssessmentResult.FromResult(result);

                int index = 0;
                foreach (var word in pronResult.Words)
                {
                    string cleanWord = word.Word.Trim(',', '.', ' ').ToLower();
                    string originalLetter = PhoneticToLetter.TryGetValue(cleanWord, out var mappedLetter)
                        ? mappedLetter
                        : (index < rawLetters.Count ? rawLetters[index].ToString() : cleanWord.ToUpper());

                    if (word.ErrorType == "Omission" || word.AccuracyScore < LETTER_PASS_THRESHOLD)
                    {
                        problemLetters.Add(originalLetter);
                    }

                    index++;
                }

                problemLetters = problemLetters.Distinct().ToList();

                feedbackMessage = problemLetters.Count == 0
                    ? "Great job! All letters pronounced correctly!"
                    : $"Work on these letters: {string.Join(", ", problemLetters)}.";
            }
            else
            {
                feedbackMessage = "No speech could be recognized. Speak clearly, one letter at a time.";
            }

            var attempt = new AlphabetAttempt
            {
                UserId = userId,
                AlphabetEntryId = alphabetEntryId,
                ProblemLetters = string.Join(",", problemLetters),
                Feedback = feedbackMessage,
                CreatedAt = PolandTime.DateTimeNow
            };

            _dbContext.AlphabetAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

            return new AlphabetResult
            {
                ProblemLetters = attempt.ProblemLetters,
                Feedback = feedbackMessage
            };
        }
    }
}