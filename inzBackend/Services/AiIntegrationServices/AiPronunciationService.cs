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

            using var memoryStream = new MemoryStream();
            await audioStream.CopyToAsync(memoryStream);
            byte[] audioBytes = memoryStream.ToArray();

            using var pushStream = AudioInputStream.CreatePushStream();
            pushStream.Write(audioBytes);
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
    }
}