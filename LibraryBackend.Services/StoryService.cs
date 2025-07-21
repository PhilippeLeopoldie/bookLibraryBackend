using LibraryBackend.Core.Dtos.Stories;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using Services.Contracts;


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
        var systemMessage = ChatMessage.CreateSystemMessage("create a story base on the information provided: Language, ReadingTime in minutes, Genre. Your response format: 'Title:...', 'Story:...' , 'Author: OpenAI'");
        var userMessage = ChatMessage.CreateUserMessage($"language:{prompt.language}, ReadingTime:{prompt.ReadingTime}, Genre:{prompt.Genre}");
        var messages = new ChatMessage[] { systemMessage, userMessage };
        var client = new ChatClient(_modelNames, _apiKey);
        var response = await client.CompleteChatAsync(messages, null, CancellationToken.None);
        return response.Value.Content[0].Text;
    }
}
