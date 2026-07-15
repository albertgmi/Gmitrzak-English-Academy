using inzBackend.Models.CatalogueModels;
using OpenAI.Chat;
using System.Text.Json;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiTranslationService : IAiTranslationService
    {
        private readonly ChatClient _chatClient;
        public AiTranslationService(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<List<string>> TranslateBatchAsync(List<string> texts,
            string targetLanguage = "Polish")
        {
            if (texts == null || !texts.Any())
                return new List<string>();
            var jsonInput = JsonSerializer.Serialize(new { items = texts });
            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(
                    "You are a precise dictionary translator. Translate the array of string items into the requested target language. " +
                    "It is very importat to keep the context of product catalogs (short descriptions, names, keys). " +
                    "CRITICAL: You must return ONLY a JSON object matching this schema: { \"translations\": [\"string\", \"string\", ...] }. " +
                    "The number of translated items MUST match exactly the number of input items. Do not change the order. Do not include conversational filler."
                ),
                ChatMessage.CreateUserMessage($"Target Language: {targetLanguage}. Input JSON: {jsonInput}")
            };
            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.1f
            };
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
            string responseText = completion.Content[0].Text;
            var result = JsonSerializer.Deserialize<TranslationResult>(responseText);
            return result?.Translations ?? new List<string>();
        }

        public async Task<List<bool>> ValidateTranslationsAsync(
            List<(string Source, string Translation)> pairs,
            string targetLanguage = "Polish")
        {
            if (pairs == null || !pairs.Any())
                return new List<bool>();

            var jsonInput = JsonSerializer.Serialize(new
            {
                items = pairs.Select(p => new { source = p.Source, translation = p.Translation })
            });

            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(
                    "You are a strict translation quality checker for a product catalog / vocabulary learning app. " +
                    "For each item you receive a 'source' text and its existing 'translation' into the target language. " +
                    "Judge ONLY whether the translation is an acceptable, correct translation of the source in the given context " +
                    "(short descriptions, names, keys, or vocabulary words/sentences). Minor stylistic differences are acceptable, " +
                    "but wrong meaning, wrong word, untranslated text, or nonsense must be marked as invalid. " +
                    "CRITICAL: You must return ONLY a JSON object matching this schema: { \"valid\": [true, false, ...] }. " +
                    "The number of boolean values MUST match exactly the number of input items, in the same order. Do not include conversational filler."
                ),
                ChatMessage.CreateUserMessage($"Target Language: {targetLanguage}. Input JSON: {jsonInput}")
            };

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.1f
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
            string responseText = completion.Content[0].Text;
            var result = JsonSerializer.Deserialize<TranslationValidationResult>(responseText);

            var valid = result?.Valid ?? new List<bool>();

            if (valid.Count < pairs.Count)
            {
                var padded = new List<bool>(valid);
                padded.AddRange(Enumerable.Repeat(false, pairs.Count - valid.Count));
                return padded;
            }

            return valid;
        }
    }
}