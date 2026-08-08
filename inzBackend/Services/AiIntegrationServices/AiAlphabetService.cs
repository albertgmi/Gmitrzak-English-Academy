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
        private readonly IAiUsageGuardService _usageGuard;

        private const int LETTER_PASS_THRESHOLD = 50;

        private static readonly Dictionary<char, string> LetterNames = new()
        {
            ['A'] = "Ay",
            ['B'] = "Bee",
            ['C'] = "See",
            ['D'] = "Dee",
            ['E'] = "Ee",
            ['F'] = "Ef",
            ['G'] = "Jee",
            ['H'] = "Aitch",
            ['I'] = "Eye",
            ['J'] = "Jay",
            ['K'] = "Kay",
            ['L'] = "El",
            ['M'] = "Em",
            ['N'] = "En",
            ['O'] = "Oh",
            ['P'] = "Pee",
            ['Q'] = "Cue",
            ['R'] = "Ar",
            ['S'] = "Ess",
            ['T'] = "Tee",
            ['U'] = "You",
            ['V'] = "Vee",
            ['W'] = "Double-you",
            ['X'] = "Ex",
            ['Y'] = "Why",
            ['Z'] = "Zee",
            ['0'] = "Zero",
            ['1'] = "One",
            ['2'] = "Two",
            ['3'] = "Three",
            ['4'] = "Four",
            ['5'] = "Five",
            ['6'] = "Six",
            ['7'] = "Seven",
            ['8'] = "Eight",
            ['9'] = "Nine"
        };

        private static readonly Dictionary<string, char> ReverseLetterNames =
            LetterNames.ToDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

        public AiAlphabetService(IUserContextService userContextService, GmitrzakEnglishAcademyDbContext dbContext,
            IConfiguration configuration, IAiUsageGuardService usageGuard)
        {
            _userContextService = userContextService;
            _dbContext = dbContext;
            _azureSubscriptionKey = configuration["AzureSpeechSettings:SubscriptionKey"]
                ?? throw new InvalidOperationException("AzureSpeechSettings:SubscriptionKey is missing in configuration.");
            _azureRegion = configuration["AzureSpeechSettings:Region"]
                ?? throw new InvalidOperationException("AzureSpeechSettings:Region is missing in configuration.");
            _usageGuard = usageGuard;
        }

        public async Task<AlphabetResult> ProcessUserAttemptAsync(Stream audioStream, string fileName, int alphabetEntryId)
        {
            int userId = _userContextService.GetUserId!.Value;

            _usageGuard.EnsureCanSubmitAttempt(userId);

            var entry = await _dbContext.AlphabetEntries
                .FirstOrDefaultAsync(x => x.Id == alphabetEntryId && x.UserId == userId);

            if (entry == null)
                throw new NotFoundException("Alphabet entry not found");

            var rawChars = entry.Content
                .Where(char.IsLetterOrDigit)
                .Select(char.ToUpper)
                .ToList();

            if (!rawChars.Any())
                throw new InvalidOperationException("Alphabet entry contains no valid letters or digits.");

            var referenceWords = rawChars.Select(c => LetterNames.TryGetValue(c, out var name) ? name : c.ToString());
            var referenceText = string.Join(" ", referenceWords);

            using var memoryStream = new MemoryStream();
            await audioStream.CopyToAsync(memoryStream);
            byte[] audioBytes = memoryStream.ToArray();

            WavAudioInfo wavInfo;
            try
            {
                wavInfo = ParseWavHeader(audioBytes);
            }
            catch (InvalidOperationException)
            {
                return await SaveAndReturn(userId, alphabetEntryId, new List<string>(),
                    "Could not read the audio recording. Please try again.");
            }

            if (wavInfo.DataLength < wavInfo.SampleRate * wavInfo.BlockAlign / 4)
            {
                return await SaveAndReturn(userId, alphabetEntryId, new List<string>(),
                    "Recording was too short. Please hold the button and speak clearly.");
            }

            var speechConfig = SpeechConfig.FromSubscription(_azureSubscriptionKey, _azureRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            speechConfig.SetProperty(PropertyId.SpeechServiceConnection_EndSilenceTimeoutMs, "8000");
            speechConfig.SetProperty(PropertyId.SpeechServiceConnection_InitialSilenceTimeoutMs, "10000");

            var pcmFormat = AudioStreamFormat.GetWaveFormatPCM(
                (uint)wavInfo.SampleRate, (byte)wavInfo.BitsPerSample, (byte)wavInfo.Channels);

            using var pushStream = AudioInputStream.CreatePushStream(pcmFormat);
            pushStream.Write(audioBytes[wavInfo.DataOffset..(wavInfo.DataOffset + wavInfo.DataLength)]);
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

            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    var pronResult = PronunciationAssessmentResult.FromResult(result);

                    foreach (var word in pronResult.Words)
                    {
                        if (word.ErrorType == "Insertion") continue;

                        var cleanWord = word.Word.Trim(',', '.', ' ');
                        var originalLetter = ReverseLetterNames.TryGetValue(cleanWord, out var c)
                            ? c.ToString()
                            : cleanWord.ToUpper();

                        if (word.ErrorType == "Omission" || word.AccuracyScore < LETTER_PASS_THRESHOLD)
                            problemLetters.Add(originalLetter);
                    }

                    problemLetters = problemLetters.Distinct().ToList();

                    feedbackMessage = problemLetters.Count == 0
                        ? "Great job! All letters pronounced correctly!"
                        : $"Work on these letters: {string.Join(", ", problemLetters)}.";
                    break;

                case ResultReason.NoMatch:
                    feedbackMessage = "No speech could be recognized. Speak clearly, letter by letter.";
                    break;

                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    feedbackMessage = cancellation.Reason == CancellationReason.Error
                        ? "Speech service error - please try again in a moment."
                        : "Recognition was interrupted. Please try again.";
                    break;

                default:
                    feedbackMessage = "Could not evaluate pronunciation. Please try again.";
                    break;
            }

            return await SaveAndReturn(userId, alphabetEntryId, problemLetters, feedbackMessage);
        }

        private async Task<AlphabetResult> SaveAndReturn(
            int userId, int alphabetEntryId, List<string> problemLetters, string feedbackMessage)
        {
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

        private static WavAudioInfo ParseWavHeader(byte[] wav)
        {
            if (wav.Length < 44
                || wav[0] != 'R' || wav[1] != 'I' || wav[2] != 'F' || wav[3] != 'F'
                || wav[8] != 'W' || wav[9] != 'A' || wav[10] != 'V' || wav[11] != 'E')
            {
                throw new InvalidOperationException("Invalid WAV file.");
            }

            short channels = BitConverter.ToInt16(wav, 22);
            int sampleRate = BitConverter.ToInt32(wav, 24);
            short bitsPerSample = BitConverter.ToInt16(wav, 34);
            short blockAlign = BitConverter.ToInt16(wav, 32);

            int pos = 12;
            int dataOffset = -1, dataLength = 0;

            while (pos + 8 <= wav.Length)
            {
                int chunkSize = BitConverter.ToInt32(wav, pos + 4);
                bool isData = wav[pos] == 'd' && wav[pos + 1] == 'a' && wav[pos + 2] == 't' && wav[pos + 3] == 'a';

                if (isData)
                {
                    dataOffset = pos + 8;
                    dataLength = Math.Min(chunkSize, wav.Length - dataOffset);
                    break;
                }

                pos += 8 + chunkSize + (chunkSize % 2);
            }

            if (dataOffset < 0 || dataLength <= 0)
                throw new InvalidOperationException("WAV file has no data chunk.");

            return new WavAudioInfo
            {
                SampleRate = sampleRate,
                Channels = channels,
                BitsPerSample = bitsPerSample,
                BlockAlign = blockAlign == 0 ? (short)(channels * bitsPerSample / 8) : blockAlign,
                DataOffset = dataOffset,
                DataLength = dataLength
            };
        }
    }
}