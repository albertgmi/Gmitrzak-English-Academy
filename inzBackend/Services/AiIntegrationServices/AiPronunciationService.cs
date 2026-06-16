using GenerativeAI;
using GenerativeAI.Types;
using inzBackend.Entities.LearningMaterials;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AiPronunciationModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiPronunciationService : IAiPronunciationService
    {
        private readonly GenerativeModel _model;
        private readonly IUserContextService _userContextService;
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AiPronunciationService(GenerativeModel model,
            IUserContextService userContextService, GmitrzakEnglishAcademyDbContext dbContext)
        {
            _model = model;
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

            string prompt = $@"
                You are a strict English pronunciation coach. 
                Analyze the provided audio and compare it to the target word: '{entry.Word}'.
                Evaluate:
                1. Phonetic accuracy.
                2. Stress.
                3. Natural rhythm.
                Return ONLY raw JSON (no markdown formatting):
                {{
                    ""score"": 0,
                    ""result"": ""Great"" | ""Not yet"",
                    ""feedback"": ""short explanation""
                }}";

            var content = new Content();
            content.Parts.Add(new Part { Text = prompt });

            content.Parts.Add(new Part
            {
                InlineData = new Blob
                {
                    MimeType = "audio/wav",
                    Data = Convert.ToBase64String(audioBytes)
                }
            });
            var request = new GenerateContentRequest
            {
                Contents = new List<Content> { content }
            };
            var response = await _model.GenerateContentAsync(request);

            string responseText = response.Text.Replace("```json", "").Replace("```", "").Trim();

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