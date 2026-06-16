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

        public async Task<PronunciationResult> processUserAttemptAsync(Stream audioStream, string fileName, 
            int pronunciationEntryId)
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
                    """
                    You are a strict English pronunciation evaluator.

                    You are judging spoken pronunciation, not spelling.

                    Important:
                    The speech recognition result is NOT proof of correct pronunciation.
                    Speech recognition may automatically fix pronunciation mistakes.

                    Compare:
                    - target English word
                    - how the student likely pronounced it
                    - natural English rhythm and stress

                    Rules:
                    - Do not mark Great only because transcription matches the target word.
                    - Accept small accent differences.
                    - Reject unnatural syllable-by-syllable pronunciation.
                    - Reject wrong stress or clearly incorrect sounds.
                    - If pronunciation is understandable and very close to the expected English pronunciation -> Great.
                    - Otherwise -> Not yet.

                    Only return Great and high score when the pronunciation is likely natural.
                    When uncertain, return Not yet.

                    Scoring:
                    90-100 = natural pronunciation
                    70-89 = acceptable learner pronunciation
                    below 70 = incorrect pronunciation

                    Return ONLY JSON:
                    {
                    "score": number,
                    "result": "Great" | "Not yet",
                    "feedback": "short explanation"
                    }
                    """
                ),
                ChatMessage.CreateUserMessage(
                    $"""
                    Target word:
                    "{entry.Word}"
                    Speech recognition heard:
                    "{transcribedText}"
                    The expected pronunciation should be inferred from the target word itself.
                    Evaluate if the spoken pronunciation was natural and close enough.
                    """)
            };

            var chatOptions = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.0f
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, chatOptions);
            string responseText = completion.Content[0].Text;

            var evaluation =
                JsonSerializer.Deserialize<PronunciationEvaluationJson>(
                    responseText,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            string aiResultStatus = evaluation != null && evaluation.Score >= 70 ? "Great" : "Not yet";

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
                TranscribedText = transcribedText,
                Score = evaluation?.Score ?? 0
            };
        }
    }
}