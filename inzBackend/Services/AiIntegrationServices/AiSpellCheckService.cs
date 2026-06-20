using inzBackend.Models.AiSpellCheckingModels;
using OpenAI.Chat;
using System.Text.Json;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiSpellCheckService : IAiSpellCheckService
    {
        private readonly ChatClient _chatClient;

        public AiSpellCheckService(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<SpellCheckResult> CheckTextAsync(string text, string language = "English")
        {
            if (string.IsNullOrWhiteSpace(text))
                return new SpellCheckResult { HasError = false };

            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(
                    "You are a meticulous proofreader for a language-learning app. " +
                    "Check the given text (a single word, phrase, or sentence) for spelling, grammar, or typo errors in the specified language. " +
                    "Only flag REAL errors - typos, misspellings, wrong word forms, missing/extra letters, incorrect grammar. " +
                    "Do NOT flag stylistic choices, valid alternative spellings, slang, or informal but correct language. " +
                    "If the text is correct, set hasError to false and corrected/reason to null. " +
                    "If there is an error, set hasError to true, provide the corrected version in 'corrected', " +
                    "and a very short reason (max 6 words) in 'reason'. " +
                    "CRITICAL: Return ONLY a JSON object matching this schema: " +
                    "{ \"hasError\": boolean, \"corrected\": \"string|null\", \"reason\": \"string|null\" }. " +
                    "Do not include conversational filler."
                ),
                ChatMessage.CreateUserMessage($"Language: {language}. Text: \"{text}\"")
            };

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.1f
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
            string responseText = completion.Content[0].Text;

            var result = JsonSerializer.Deserialize<SpellCheckResult>(responseText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new SpellCheckResult { HasError = false };
        }

        public async Task<List<SpellCheckResult>> CheckBatchAsync(List<SpellCheckRequestItem> items)
        {
            if (items == null || !items.Any())
                return new List<SpellCheckResult>();

            var jsonInput = JsonSerializer.Serialize(new
            {
                items = items.Select(i => new { text = i.Text, language = i.Language })
            });

            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(
                    "You are a meticulous proofreader for a language-learning app. " +
                    "For each item, check the 'text' for spelling, grammar, or typo errors in the specified 'language'. " +
                    "Only flag REAL errors - typos, misspellings, wrong word forms, missing/extra letters, incorrect grammar. " +
                    "Do NOT flag stylistic choices, valid alternative spellings, slang, or informal but correct language. " +
                    "CRITICAL: Return ONLY a JSON object matching this schema: " +
                    "{ \"results\": [ { \"hasError\": boolean, \"corrected\": \"string|null\", \"reason\": \"string|null\" }, ... ] }. " +
                    "The number of results MUST match exactly the number of input items, in the same order. " +
                    "Do not include conversational filler."
                ),
                ChatMessage.CreateUserMessage($"Input JSON: {jsonInput}")
            };

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.1f
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
            string responseText = completion.Content[0].Text;

            var result = JsonSerializer.Deserialize<SpellCheckBatchResult>(responseText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Results ?? new List<SpellCheckResult>();
        }
    }
}
