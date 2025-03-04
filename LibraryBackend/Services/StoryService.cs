using OpenAI.Chat;
using LibraryBackend.Models;
using System.Text;

namespace LibraryBackend.Services;

public class StoryService : IStoryService
{
    private readonly string _apiKey;
    private readonly string _modelNames;

    public StoryService(IConfiguration configuration)
    {
        _apiKey = configuration["OPENAI_API_KEY"] ??
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new ArgumentNullException(nameof(configuration));
        _modelNames = "gpt-4o-mini";
    }

    public async Task<string> GenerateAIStoryAsync(StoryDtoRequest prompt)
    {
        var systemMessage = ChatMessage.CreateSystemMessage("create a story based on the information provided: ReadingTime in minutes, Genre");
        var userMessage = ChatMessage.CreateUserMessage($"ReadingTime:{prompt.ReadingTime},Genre:{prompt.Genre}");
        var messages = new ChatMessage[] { systemMessage, userMessage };
        var client = new ChatClient(_modelNames, _apiKey);

        var AiStreamResponse =  client.CompleteChatStreamingAsync(messages, null, CancellationToken.None);

        var story = new StringBuilder();
        await foreach (var update in AiStreamResponse)
        {
            foreach  (var content in update.ContentUpdate)
            {
                story.Append(content.Text);
            }
        }
        return story.ToString();
    }
}
