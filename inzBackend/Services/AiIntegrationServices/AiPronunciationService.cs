using inzBackend.Entities.LearningMaterials;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using OpenAI.Audio;
using OpenAI.Chat;
using System.Text.Json;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiPronunciationService : IAiPronunciationService
    {
        private readonly AudioClient _audioClient;
        private readonly ChatClient _chatClient;
        private readonly IUserContextService _userContextService;
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AiPronunciationService(AudioClient audioClient, ChatClient chatClient,
            IUserContextService userContextService, GmitrzakEnglishAcademyDbContext dbContext)
        {
            _audioClient = audioClient;
            _chatClient = chatClient;
            _userContextService = userContextService;
            _dbContext = dbContext;
        }

        public async Task<PronunciationResult> processUserAttemptAsync(Stream audioStream, string fileName, int pronunciationEntryId)
        {
            int userId = _userContextService.GetUserId!.Value;

            var entry = await _dbContext.PronunciationEntries
                .FirstOrDefaultAsync(x => x.Id == pronunciationEntryId && x.UserId == userId);

            if (entry == null)
                throw new NotFoundException("Pronunciation entry not found");

            var transcriptionOptions = new AudioTranscriptionOptions
            {
                Language = "en",
                Temperature = 0.0f
            };

            AudioTranscription transcription = await _audioClient.TranscribeAudioAsync(audioStream, fileName, transcriptionOptions);
            string transcribedText = transcription.Text?.Trim() ?? string.Empty;

            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(
                    "You are an expert pronunciation evaluator for a language-learning app. " +
                    "Compare the 'Expected text' (what the student was supposed to say) with the 'Transcribed text' (what the speech-to-text system actually heard). " +
                    "Evaluate if the student's pronunciation was close enough to be considered correct. " +
                    "CRITICAL: Return ONLY a JSON object: { \"result\": \"Great\" | \"Not yet\" }. " +
                    "Result rules: 'Great' if the texts match or are phonetically near-identical. 'Not yet' if the word was mispronounced heavily, skipped, or misheard as a different word."
                ),
                ChatMessage.CreateUserMessage(
                    $"Expected text: \"{entry.Word}\"\n" +
                    $"Transcribed text: \"{transcribedText}\""
                )
            };

            var chatOptions = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.1f
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, chatOptions);
            string responseText = completion.Content[0].Text;

            var evaluation = JsonSerializer.Deserialize<PronunciationEvaluationJson>(responseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var aiResultStatus = evaluation?.Result ?? "Not yet";

            var attempt = new PronunciationAttempt
            {
                UserId = userId,
                PronunciationEntryId = pronunciationEntryId,
                TranscribedText = transcribedText,
                Result = aiResultStatus,
                CreatedAt = PolandTime.DateTimeNow
            };

            _dbContext.PronunciationAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

            return new PronunciationResult
            {
                Result = aiResultStatus,
                TranscribedText = transcribedText
            };
        }
    }
}