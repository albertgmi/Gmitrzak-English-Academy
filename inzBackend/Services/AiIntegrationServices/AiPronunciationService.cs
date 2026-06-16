using Google.GenAI;
using Google.GenAI.Types;
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
        private readonly Client _client;
        private readonly IUserContextService _userContextService;
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AiPronunciationService(Client client, IUserContextService userContextService,
            GmitrzakEnglishAcademyDbContext dbContext)
        {
            _client = client;
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
                 
                Feedback must be around 15 words long.

                Return ONLY raw JSON:
                {{
                    ""score"": 0,
                    ""result"": ""Great"" | ""Not yet"",
                    ""feedback"": ""short explanation""
                }}";

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-2.5-flash-lite",
                contents: new List<Content>
                {
                    new Content
                    {
                        Parts =
                        [
                            new Part { Text = prompt },
                            new Part
                            {
                                InlineData = new Blob
                                {
                                    MimeType = "audio/wav",
                                    Data = audioBytes
                                }
                            }
                        ]
                    }
                });

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