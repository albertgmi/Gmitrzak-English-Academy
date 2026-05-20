using OpenAI.Chat;
using System.Text.Json;

namespace inzBackend.Services.AiIntegrationServices
{
    public class AiSentenceCheckerService : IAiSentenceCheckerService
    {
        private readonly ChatClient _chatClient;

        public AiSentenceCheckerService(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<(string result, string explanation)> CheckAnswerAsync(
            string polish, string expectedEnglish, string userAnswer)
        {
            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(
                    "You are an English language teacher checking student translations. " +
                    "Evaluate whether the student's answer is correct. " +
                    "Accept synonyms and paraphrases that preserve the original meaning and are grammatically correct. " +
                    "CRITICAL: Return ONLY a JSON object: { \"result\": \"Correct\" | \"Partial\" | \"Incorrect\", \"explanation\": \"brief explanation\" }. " +
                    "Result: Correct = fully correct meaning and grammar. Partial = meaning OK but grammar issues or minor errors. Incorrect = wrong meaning or major errors."
                ),
                ChatMessage.CreateUserMessage(
                    $"Polish sentence: {polish}\n" +
                    $"Expected English: {expectedEnglish}\n" +
                    $"Student answer: {userAnswer}"
                )
            };

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.1f
            };

            var completion = await _chatClient.CompleteChatAsync(messages, options);
            var json = completion.Value.Content[0].Text;

            using var doc = JsonDocument.Parse(json);
            var result = doc.RootElement.GetProperty("result").GetString() ?? "Incorrect";
            var explanation = doc.RootElement.GetProperty("explanation").GetString() ?? string.Empty;

            return (result, explanation);
        }
    }
}
