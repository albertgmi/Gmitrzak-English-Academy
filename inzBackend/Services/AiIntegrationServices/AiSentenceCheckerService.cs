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
                    "You are a friendly English language teacher checking student translations from Polish to English. " +
                    "Evaluate whether the student's answer is correct. " +
                    "Accept synonyms and paraphrases that preserve the original meaning and are grammatically correct. " +
                    "CRITICAL: Return ONLY a JSON object: { \"result\": \"Correct\" | \"Partial\" | \"Incorrect\", \"explanation\": \"string\" }. " +
                    "Result rules: Correct = fully correct meaning and grammar. Partial = meaning OK but has grammar issues or minor errors. Incorrect = wrong meaning or major errors. " +
                    "Explanation rules: Write directly to the student in second person (you/your). Be concise (1-2 sentences max). " +
                    "For Correct: give brief positive feedback, e.g. 'Great job! Your translation is accurate and grammatically correct.' " +
                    "For Partial: explain the specific grammar mistake or minor error and show the correct form, e.g. 'Good meaning, but you used past tense instead of present perfect. It should be \"have gone\" instead of \"went\".' " +
                    "For Incorrect: clearly explain what was wrong with the meaning or structure and show what was expected, e.g. 'You translated the wrong subject. The sentence is about a car, not a person. The correct translation is: ...' " +
                    "Always write the explanation in English."
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
