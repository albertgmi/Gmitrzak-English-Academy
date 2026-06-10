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
    }
}
