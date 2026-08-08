using inzBackend.Entities.LearningMaterials;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Services.UserServices;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiPronunciationService : IAiPronunciationService
    {
        private readonly IUserContextService _userContextService;
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly string _azureSubscriptionKey;
        private readonly string _azureRegion;
        private readonly IAiUsageGuardService _usageGuard;

        public AiPronunciationService(IUserContextService userContextService, GmitrzakEnglishAcademyDbContext dbContext,
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

        public async Task<PronunciationResult> ProcessUserAttemptAsync(Stream audioStream, string fileName, int pronunciationEntryId)
        {
            int userId = _userContextService.GetUserId!.Value;

            _usageGuard.EnsureCanSubmitAttempt(userId);

            var entry = await _dbContext.PronunciationEntries
                .FirstOrDefaultAsync(x => x.Id == pronunciationEntryId && x.UserId == userId);

            if (entry == null)
                throw new NotFoundException("Pronunciation entry not found");

            var speechConfig = SpeechConfig.FromSubscription(_azureSubscriptionKey, _azureRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";
            speechConfig.SetProperty(PropertyId.SpeechServiceConnection_EndSilenceTimeoutMs, "8000");
            speechConfig.SetProperty(PropertyId.SpeechServiceConnection_InitialSilenceTimeoutMs, "10000");

            using var memoryStream = new MemoryStream();
            await audioStream.CopyToAsync(memoryStream);
            byte[] audioBytes = memoryStream.ToArray();

            if (audioBytes.Length < 44)
            {
                throw new InvalidOperationException("Recording was too short. Please hold the button and speak clearly.");
            }

            WavAudioInfo wavInfo = ParseWavHeader(audioBytes);

            var pcmFormat = AudioStreamFormat.GetWaveFormatPCM(
                (uint)wavInfo.SampleRate, (byte)wavInfo.BitsPerSample, (byte)wavInfo.Channels);

            using var pushStream = AudioInputStream.CreatePushStream(pcmFormat);
            pushStream.Write(audioBytes[wavInfo.DataOffset..(wavInfo.DataOffset + wavInfo.DataLength)]);
            pushStream.Close();

            using var audioConfig = AudioConfig.FromStreamInput(pushStream);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var pronConfig = new PronunciationAssessmentConfig(
                referenceText: entry.Word,
                gradingSystem: GradingSystem.HundredMark,
                granularity: Granularity.Phoneme,
                enableMiscue: true
            );
            pronConfig.EnableProsodyAssessment();
            pronConfig.ApplyTo(recognizer);

            var result = await recognizer.RecognizeOnceAsync();

            int finalScore = 0;
            string finalResultStatus = "Not yet";
            string feedbackMessage = "Could not evaluate pronunciation. Please try again.";
            var phonemeList = new List<PhonemeAssessmentDto>();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                var pronResult = PronunciationAssessmentResult.FromResult(result);

                finalScore = (int)Math.Round(pronResult.AccuracyScore);
                finalResultStatus = finalScore >= 75 ? "Great" : "Not yet";
                feedbackMessage = BuildDetailedFeedback(pronResult);

                foreach (var word in pronResult.Words)
                {
                    foreach (var phoneme in word.Phonemes)
                    {
                        phonemeList.Add(new PhonemeAssessmentDto
                        {
                            Phoneme = phoneme.Phoneme,
                            IsCorrect = phoneme.AccuracyScore >= 60
                        });
                    }
                }
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                feedbackMessage = "No speech could be recognized. Speak clearly into the microphone.";
            }

            var attempt = new PronunciationAttempt
            {
                UserId = userId,
                PronunciationEntryId = pronunciationEntryId,
                Feedback = feedbackMessage,
                Result = finalResultStatus,
                Score = finalScore,
                CreatedAt = PolandTime.DateTimeNow
            };

            _dbContext.PronunciationAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

            return new PronunciationResult
            {
                Result = finalResultStatus,
                Feedback = feedbackMessage,
                Score = finalScore,
                Phonemes = phonemeList
            };
        }

        private static string BuildDetailedFeedback(PronunciationAssessmentResult pronResult)
        {
            var mispronouncedPhonemes = new List<string>();

            foreach (var word in pronResult.Words)
            {
                if (word.ErrorType == "Omission")
                {
                    return $"You omitted the word or sound in '{word.Word}'.";
                }

                foreach (var phoneme in word.Phonemes)
                {
                    if (phoneme.AccuracyScore < 60)
                    {
                        mispronouncedPhonemes.Add($"/{phoneme.Phoneme}/");
                    }
                }
            }

            if (mispronouncedPhonemes.Count > 0)
            {
                var distinctPhonemes = mispronouncedPhonemes.Distinct();
                return $"Pay attention to the sound: {string.Join(", ", distinctPhonemes)}.";
            }

            if (pronResult.AccuracyScore >= 85)
            {
                return "Excellent pronunciation!";
            }

            return "Good attempt, but try to speak more clearly.";
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