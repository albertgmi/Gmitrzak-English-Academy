using inzBackend.Entities.LearningMaterials;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using System.Text.Json;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiPronunciationService : IAiPronunciationService
    {
        private readonly ChatClient _chatClient;
        private readonly IUserContextService _userContextService;
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AiPronunciationService(ChatClient chatClient,
            IUserContextService userContextService, GmitrzakEnglishAcademyDbContext dbContext)
        {
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

            using var memoryStream = new MemoryStream();
            await audioStream.CopyToAsync(memoryStream);
            byte[] audioBytes = memoryStream.ToArray();

#pragma warning disable OPENAI001
            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(
                    """
                    You are a strict English pronunciation coach. 
                    You are analyzing the provided raw audio file. 
                    Do NOT rely on auto-transcription. Listen to the audio and compare it to the target word.
                    
                    Evaluate:
                    1. Phonetic accuracy.
                    2. Stress (e.g., 'com-for-ta-ble' vs 'comfortable').
                    3. Natural rhythm.

                    Return ONLY JSON:
                    {
                        "score": number (0-100),
                        "result": "Great" | "Not yet",
                        "feedback": "short explanation focusing on why it sounds natural or unnatural"
                    }
                    """
                ),
                ChatMessage.CreateUserMessage(
                    ChatMessageContentPart.CreateTextPart($"Target word: {entry.Word}. Analyze the pronunciation of the following audio:"),
                    ChatMessageContentPart.CreateInputAudioPart(BinaryData.FromBytes(audioBytes), "audio/wav")
                )
            };
#pragma warning restore OPENAI001

            var chatOptions = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.0f
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, chatOptions);
            string responseText = completion.Content[0].Text;

            var evaluation = JsonSerializer.Deserialize<PronunciationEvaluationJson>(responseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var attempt = new PronunciationAttempt
            {
                UserId = userId,
                PronunciationEntryId = pronunciationEntryId,
                Feedback = evaluation?.Feedback ?? "No feedback",
                Result = evaluation?.Result ?? "Not yet",
                CreatedAt = PolandTime.DateTimeNow
            };

            _dbContext.PronunciationAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

            return new PronunciationResult
            {
                Result = evaluation?.Result ?? "Not yet",
                Feedback = evaluation?.Feedback ?? "No feedback",
                Score = evaluation?.Score ?? 0
            };
        }
    }
}