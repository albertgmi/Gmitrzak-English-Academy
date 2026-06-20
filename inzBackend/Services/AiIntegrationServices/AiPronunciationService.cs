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

        public async Task<PronunciationResult> ProcessUserAttemptAsync(Stream audioStream, string fileName,
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
                You are an elite expert in English phonetics and linguistics.
                Your task is to perform an ultra-precise audit of the user's audio against the target word: '{entry.Word}'.

                STRICT ANALYSIS PROTOCOL:
                1. Phoneme Precision: Even a single mispronounced consonant, vowel, or missing suffix counts as a major error.
                2. Detailed Audit: Evaluate if every individual sound matches native pronunciation.
                3. Penalty System: If the user misses even one letter/sound (e.g., 's' at the end, wrong vowel), the score must be below 60%.
                4. Feedback Focus: Point out the exact sound/letter that caused the failure.

                Return ONLY raw JSON. Do not include any conversational filler.
                {{
                    ""score"": 0-100,
                    ""result"": ""Great"" (if 75-100), ""Not yet"" (if < 75),
                    ""feedback"": ""Identify the specific missing/wrong letter or sound. Max 15 words.""
                }}";

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3.1-flash-lite",
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
                Score = evaluation?.Score ?? 0,
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